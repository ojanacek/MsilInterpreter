using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MsilInterpreterLib.Components;
using MsilInterpreterLib.Msil;

namespace MsilInterpreterLib
{
    internal class Interpreter
    {
        private readonly Runtime runtime;

        public Runtime Runtime { get { return runtime; } }
        public StackFrame CurrentStackFrame { get { return Runtime.CallStack.Peek(); } }

        public Interpreter(Runtime runtime)
        {
            this.runtime = runtime;
        }

        public void Execute(DotMethodBase method)
        {
            PushToCallStack(method);

            if (method.Body.Count == 0)
                method.Execute(this);
            else
                Interpret(method.Body);
            
            UnwindCallStack();
        }

        private void Interpret(IEnumerable<ILInstruction> methodBody)
        {
            var instructions = methodBody.ToList();
            var offsetToIndexMapping = instructions.Select((instruction, index) => new { instruction.Offset, Index = index })
                                                   .ToDictionary(x => x.Offset, x => x.Index);

            var flowIndexer = 0;
            while (flowIndexer < instructions.Count)
            {
                var currentInstruction = instructions[flowIndexer];

                switch (currentInstruction.Code.FlowControl)
                {
                    case FlowControl.Branch:
                    case FlowControl.Cond_Branch:
                        var jumpTo = InterpretBranchInstruction(currentInstruction);
                        if (jumpTo == -1)
                        {
                            flowIndexer++;
                            continue;
                        }
                        flowIndexer = offsetToIndexMapping[jumpTo];
                        break;
                    case FlowControl.Break:
                        throw new NotSupportedException(currentInstruction.ToString());
                    case FlowControl.Call:
                        InterpretCallInstruction(currentInstruction);
                        flowIndexer++;
                        break;
                    case FlowControl.Meta:
                        throw new NotSupportedException(currentInstruction.ToString());
                    case FlowControl.Next:
                        InterpretNextInstruction(currentInstruction);
                        flowIndexer++;
                        break;
                    case FlowControl.Return:
                        if (currentInstruction.Code.Name == "ret")
                            return;
                        throw new NotSupportedException(currentInstruction.ToString());
                    case FlowControl.Throw:
                        throw new NotSupportedException(currentInstruction.ToString());
                }
            }
        }

        private void InterpretNextInstruction(ILInstruction instruction)
        {
            switch (instruction.Code.Name)
            {
                case "add":
                {
                    dynamic op2 = PopFromStack();
                    dynamic op1 = PopFromStack();
                    PushToStack(op1 + op2);
                    break;
                }
                case "ceq":
                {
                    dynamic op2 = PopFromStack();
                    dynamic op1 = PopFromStack();
                    PushToStack(op1 == op2 ? 1 : 0);
                    break;
                }
                case "cgt":
                {
                    dynamic op2 = PopFromStack();
                    dynamic op1 = PopFromStack();
                    PushToStack(op1 > op2 ? 1 : 0);
                    break;
                }
                case "clt":
                {
                    dynamic value2 = PopFromStack();
                    dynamic value1 = PopFromStack();
                    PushToStack(value1 < value2 ? 1 : 0);
                    break;
                }
                case "conv.i4":
                {
                    var value = PopFromStack();
                    PushToStack(Convert.ToInt32(value));
                    break;
                }
                case "div":
                {
                    dynamic op2 = PopFromStack();
                    dynamic op1 = PopFromStack();
                    PushToStack(op1 / op2);
                    break;
                }
                case "isinst":
                {
                    var objReference = PopFromStack();
                    // If the object reference itself is a null reference, then isinst likewise returns a null reference.
                    if (objReference == null)
                        PushToStack(null);

                    var objType = GetFromHeap((Guid) objReference).TypeHandler;
                    var possibleConversions = new List<DotType> { objType }; // TODO: test also against its base classes and interfaces
                    var testAgainstType = LookUpType(instruction.Operand as Type);
                    var result = possibleConversions.Contains(testAgainstType) ? objReference : null; // TODO: return casted reference
                    PushToStack(result);
                    break;
                }
                case "ldarg.0":
                case "ldarg.1":
                case "ldarg.2":
                case "ldarg.3":
                {
                    var argPosition = Convert.ToInt32(instruction.Code.Name.Split('.')[1]);
                    var paramPosition = argPosition;

                    if (!CurrentStackFrame.CurrentMethod.IsStatic)
                    {
                        if (argPosition > 0)
                            paramPosition--; // instance methods and ctors have the first parameter (instance they are being called on) hidden
                        else
                        {
                            PushToStack(CurrentStackFrame.Arguments[0]);
                            break;
                        }
                    }

                    var param = CurrentStackFrame.CurrentMethod.ParametersTypes[paramPosition];
                    if (param.IsValueType)
                    {
                        PushToStack(CurrentStackFrame.Arguments[argPosition]);
                    }
                    else
                    {
                        var reference = (Guid)CurrentStackFrame.Arguments[argPosition];
                        PushToStack(reference);
                    }
                    
                    break;
                }
                case "ldc.i4.0": PushToStack(0); break;
                case "ldc.i4.1": PushToStack(1); break;
                case "ldc.i4.2": PushToStack(2); break;
                case "ldc.i4.3": PushToStack(3); break;
                case "ldc.i4.4": PushToStack(4); break;
                case "ldc.i4.5": PushToStack(5); break;
                case "ldc.i4.6": PushToStack(6); break;
                case "ldc.i4.7": PushToStack(7); break;
                case "ldc.i4.8": PushToStack(8); break;
                case "ldc.i4.m1": PushToStack(-1); break;
                case "ldc.i4.s": PushToStack(Convert.ToInt32(instruction.Operand)); break;
                case "ldelem.i1":
                case "ldelem.i2":
                case "ldelem.i4":
                {
                    int index = (int) PopFromStack();
                    var arrayReference = (Guid) PopFromStack();
                    var array = GetFromHeap(arrayReference)["Values"] as int[];
                    PushToStack(array[index]);
                    break;
                }
                case "ldelem.ref":
                {
                    int index = (int)PopFromStack();
                    var arrayObjRef = (Guid)PopFromStack();
                    var array = GetFromHeap(arrayObjRef)["Values"] as Guid[];
                    PushToStack(array[index]);
                    break;
                }
                case "ldfld":
                {
                    var instanceRef = PopFromStack();
                    var instance = GetFromHeap((Guid)instanceRef);
                    var fieldName = (instruction.Operand as FieldInfo).Name;
                    PushToStack(instance[fieldName]);
                    break;
                }
                case "ldlen":
                {
                    var arrayRef = PopFromStack();
                    var arrayInstance = GetFromHeap((Guid) arrayRef);
                    PushToStack(arrayInstance["Length"]);
                    break;
                }
                case "ldloc.0": PushLocalToStack(0); break;
                case "ldloc.1": PushLocalToStack(1); break;
                case "ldloc.2": PushLocalToStack(2); break;
                case "ldloc.3": PushLocalToStack(3); break;
                case "ldloc.s": PushLocalToStack((byte) instruction.Operand); break;
                case "ldstr":
                {
                    ObjectInstance stringInstance;
                    var reference = CreateObjectInstance(LookUpType(typeof(string)), out stringInstance);
                    stringInstance["Value"] = instruction.Operand;
                    PushToStack(reference);
                    break;
                }
                case "newarr":
                {
                    ObjectInstance arrayInstance;
                    var reference = CreateObjectInstance(LookUpType(typeof(Array)), out arrayInstance);
                    var elementType = (Type) instruction.Operand;

                    int size = (int)PopFromStack();
                    arrayInstance["Length"] = size;
                    if (elementType.IsValueType)
                    {
                        var array = Array.CreateInstance(elementType, size);
                        arrayInstance["Values"] = array;
                    }
                    else
                    {
                        var array = new Guid[size];
                        arrayInstance["Values"] = array;
                    }

                    PushToStack(reference);
                    break;
                }
                case "nop": break;
                case "stelem.i1":
                case "stelem.i2":
                case "stelem.i4":
                {
                    int value = (int)PopFromStack();
                    int index = (int)PopFromStack();
                    var arrayReference = (Guid)PopFromStack();
                    var array = GetFromHeap(arrayReference)["Values"] as int[];
                    array[index] = value;
                    break;
                }
                case "stfld":
                {
                    var newFieldValue = PopFromStack();
                    var instanceRef = PopFromStack();
                    var instance = GetFromHeap((Guid)instanceRef);
                    var fieldName = (instruction.Operand as FieldInfo).Name;
                    instance[fieldName] = newFieldValue;
                    break;
                }
                case "stloc.0": PopFromStackToLocal(0); break;
                case "stloc.1": PopFromStackToLocal(1); break;
                case "stloc.2": PopFromStackToLocal(2); break;
                case "stloc.3": PopFromStackToLocal(3); break;
                case "stloc.s": PopFromStackToLocal((byte)instruction.Operand); break;
                case "sub":
                {
                    dynamic op2 = PopFromStack();
                    dynamic op1 = PopFromStack();
                    PushToStack(op1 - op2);
                    break;
                }
                default:
                    throw new NotImplementedException(instruction.Code.Name + " is not implemented.");
            }
        }

        private void InterpretCallInstruction(ILInstruction instruction)
        {
            switch (instruction.Code.Name)
            {
                case "call":
                case "callvirt":
                {
                    var method = LookUpMethod(instruction.Operand as MethodBase);
                    var nestedInterp = new Interpreter(Runtime);
                    nestedInterp.Execute(method);
                    break;
                }
                case "newobj":
                {
                    var ctor = LookUpMethod(instruction.Operand as ConstructorInfo);
                    var newObjReference = (ctor as DotConstructor).Invoke(this);
                    PushToStack(newObjReference);
                    var nestedInterp = new Interpreter(Runtime);
                    nestedInterp.Execute(ctor);
                    PushToStack(newObjReference);
                    break;
                }
                default:
                    throw new NotImplementedException(instruction.Code.Name + " is not implemented.");
            }
        }

        private int InterpretBranchInstruction(ILInstruction instruction)
        {
            switch (instruction.Code.Name)
            {
                case "br":
                case "br.s":
                    return (int)instruction.Operand;
                case "brfalse":
                case "brfalse.s":
                    return (int)PopFromStack() == 0 ? (int)instruction.Operand : -1;
                case "brtrue":
                case "brtrue.s":
                    return (int)PopFromStack() == 1 ? (int)instruction.Operand : -1;
                default:
                    throw new NotImplementedException(instruction.Code.Name + " is not implemented.");
            }
        }

        #region Stack, heap and locals manipulation

        #region Call stack

        private void PushToCallStack(DotMethodBase callee)
        {
            CheckFullCallStack();

            var currentlyExecutingMethod = CurrentStackFrame.CurrentMethod;
            var newFrame = new StackFrame(currentlyExecutingMethod, callee);

            object instanceRef = null;
            if (!callee.IsStatic)
                instanceRef = PopFromStack();

            var arguments = new List<object>();
            foreach (var param in callee.ParametersTypes)
            {
                arguments.Add(PopFromStack());
            }

            if (instanceRef != null)
            {
                arguments.Insert(0, instanceRef); // instance methods and ctors have the first parameter (an instance they are being called on) hidden
                if (arguments.Count > 1)
                    VerifyArgumentOrder(arguments, callee);
            }

            newFrame.Arguments = arguments;
            Runtime.CallStack.Push(newFrame);
        }

        private void CheckFullCallStack()
        {
            // limited depth of the call stack instead of the stack size, just to make a point to throw a StackOverflow exception
            if (Runtime.CallStack.Count == 25)
                throw new StackOverflowException("There's too many nested calls (25 is the limit).");
        }

        private void VerifyArgumentOrder(List<object> arguments, DotMethodBase callee)
        {
            // sometimes arguments for instance methods and ctors are reversed, check this anomaly ... TODO: probably could be solved another way, this is a quick fix
            if (!(arguments[0] is Guid))
            {
                arguments.Reverse();
                return;
            }

            var instance = GetFromHeap((Guid) arguments[0]);
            if (instance.TypeHandler != callee.DeclaringType)
                arguments.Reverse();
        }

        private void UnwindCallStack()
        {
            var frame = Runtime.CallStack.Pop();
            
            var method = frame.CurrentMethod as DotMethod;
            if (method != null && method.ReturnType != typeof(void))
            {
                if (frame.Stack.Count == 0)
                    throw new InvalidOperationException("Can't return a value from a method call, the stack is empty.");

                CurrentStackFrame.Stack.Push(frame.Stack.Pop());
            }
        }

        #endregion

        #region Current method call stack and locals

        internal void PushToStack(object value)
        {
            CurrentStackFrame.Stack.Push(value);
        }

        internal object PopFromStack()
        {
            return CurrentStackFrame.Stack.Pop();
        }

        private void PushLocalToStack(byte index)
        {
            PushToStack(CurrentStackFrame.Locals[index]);
        }

        private void PopFromStackToLocal(byte index)
        {
            CurrentStackFrame.Locals[index] = CurrentStackFrame.Stack.Pop();
        }

        #endregion

        #region GC heap

        internal Guid CreateObjectInstance(DotType typeHandler, out ObjectInstance instance)
        {
            instance = new ObjectInstance(typeHandler);
            return Runtime.Heap.Store(instance);
        }

        internal ObjectInstance GetFromHeap(Guid reference)
        {
            return Runtime.Heap.Get(reference);
        }

        internal Guid CreateRefTypeArray(object[] sourceArray)
        {
            ObjectInstance arrayInstance;
            var arrayRef = CreateObjectInstance(LookUpType(typeof(Array)), out arrayInstance);
            var array = new Guid[sourceArray.Length];
            arrayInstance["Values"] = array;

            var elemType = sourceArray.GetType().GetElementType();
            for (int i = 0; i < sourceArray.Length; i++)
            {
                ObjectInstance elementInstance;
                var elementRef = CreateObjectInstance(LookUpType(elemType), out elementInstance);
                elementInstance["Value"] = sourceArray[i];

                array[i] = elementRef;
            }
            arrayInstance["Length"] = sourceArray.Length;
            return arrayRef;
        }

        #endregion

        #region Method Tables lookups

        private DotType LookUpType(Type type)
        {
            var moduleName = type.Module.Name.Substring(0, type.Module.Name.Length - 4); // removes a file extension .exe or .dll

            var assembly = Runtime.LoadedAssemblies.FirstOrDefault(a => a.Name == moduleName);
            if (assembly == null)
                throw new NotSupportedException("Not supported assembly " + moduleName);

            var dotType = assembly.Types.FirstOrDefault(t => t.Name == type.Name);
            if (dotType == null)
                throw new NotSupportedException("Not supported type " + type.Name + " in assembly " + moduleName);

            return dotType;
        }

        private DotMethodBase LookUpMethod(MethodBase mb)
        {
            var type = LookUpType(mb.DeclaringType);

            if (mb.IsConstructor)
            {
                var ctor = type.Constructors.FirstOrDefault(c => c.ParametersTypes.SequenceEqual(mb.GetParameters().Select(p => p.ParameterType)));
                if (ctor == null) throw new NotSupportedException("Not supported constructor " + mb.Name + " in type " + mb.DeclaringType.Name + " in assembly " + mb.Module.Name);
                return ctor;
            }

            var method = type.Methods.FirstOrDefault(m => m.Name == mb.Name);
            if (method == null) throw new NotSupportedException("Not supported method " + mb.Name + " in type " + mb.DeclaringType.Name + " in assembly " + mb.Module.Name);
            return method;
        }

        #endregion
        
        #endregion

    }
}
 