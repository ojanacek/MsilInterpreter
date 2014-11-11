using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MsilInterpreterLib
{
    public class Interpreter
    {
        private static readonly ILParser parser = new ILParser();
        private readonly Stack<object> stack = new Stack<object>();
        private readonly object[] locals = new object[255];

        internal static ILParser Parser { get { return parser; } } 
        internal Heap Heap { get; private set; }
        /// <summary>
        /// Using a stack of objects removes necessity to cast stored values to the tightest representation.
        /// For example, ldc.i4.s loads Int32 like ldc.i4 does instead of byte because casting it to object is causing overhead no matter which type. 
        /// </summary>
        internal Stack<object> Stack { get { return stack; } }
        internal object[] Locals { get { return locals; } }

        private Interpreter(Heap heap)
        {
            Heap = heap;
        }

        public Interpreter()
        {
            Heap = new Heap(10, 100);
        }

        public void Run(Action action)
        {
            Run(action.GetMethodInfo(), false);
        }

        public object Run<T>(Func<T> function)
        {
            return Run(function.GetMethodInfo(), true);
        }

        private object Run(MethodInfo methodInfo, bool expectResult)
        {
            var instructions = Parser.ParseILFromMethod(methodInfo).ToList();
            if (!instructions.Any())
                return null;

            Interpret(instructions);
            if (expectResult)
                return Stack.Pop();

            return null;
        }

        private void Interpret(List<ILInstruction> instructions)
        {
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
                        throw new NotImplementedException(currentInstruction + " is not supported yet");
                    case FlowControl.Call:
                        InterpretCallInstruction(currentInstruction);
                        flowIndexer++;
                        break;
                    case FlowControl.Meta:
                        throw new NotImplementedException(currentInstruction + " is not supported yet");
                    case FlowControl.Next:
                        InterpretNextInstruction(currentInstruction);
                        flowIndexer++;
                        break;
                    case FlowControl.Return:
                        if (currentInstruction.Code.Name == "ret")
                            return;
                        throw new NotImplementedException(currentInstruction + " is not supported yet");
                    case FlowControl.Throw:
                        throw new NotImplementedException(currentInstruction + " is not supported yet");
                }
            }
        }

        private void InterpretNextInstruction(ILInstruction instruction)
        {
            switch (instruction.Code.Name)
            {
                case "add":
                {
                    dynamic v2 = Stack.Pop();
                    dynamic v1 = Stack.Pop();
                    Stack.Push(v1 + v2);
                    break;
                }
                case "ceq":
                {
                    dynamic v2 = Stack.Pop();
                    dynamic v1 = Stack.Pop();
                    Stack.Push(v1 == v2 ? 1 : 0);
                    break;
                }
                case "cgt":
                {
                    dynamic v2 = Stack.Pop();
                    dynamic v1 = Stack.Pop();
                    Stack.Push(v1 > v2 ? 1 : 0);
                    break;
                }
                case "clt":
                {
                    dynamic v2 = Stack.Pop();
                    dynamic v1 = Stack.Pop();
                    Stack.Push(v1 < v2 ? 1 : 0);
                    break;
                }
                case "div":
                {
                    dynamic v2 = Stack.Pop();
                    dynamic v1 = Stack.Pop();
                    Stack.Push(v1 / v2);
                    break;
                }
                case "ldarg.0":
                    if (instruction.Operand != null)
                        Stack.Push(instruction.Operand);
                    break;
                case "ldc.i4.0": Stack.Push(0); break;
                case "ldc.i4.1": Stack.Push(1); break;
                case "ldc.i4.2": Stack.Push(2); break;
                case "ldc.i4.3": Stack.Push(3); break;
                case "ldc.i4.4": Stack.Push(4); break;
                case "ldc.i4.5": Stack.Push(5); break;
                case "ldc.i4.6": Stack.Push(6); break;
                case "ldc.i4.7": Stack.Push(7); break;
                case "ldc.i4.8": Stack.Push(8); break;
                case "ldc.i4.m1": Stack.Push(-1); break;
                case "ldc.i4.s": Stack.Push(Convert.ToInt32(instruction.Operand)); break;
                case "ldc.i8":
                case "ldc.r4":
                case "ldc.r8": Stack.Push(instruction.Operand); break;
                case "ldelem.i1":
                case "ldelem.i2":
                case "ldelem.i4":
                {
                    int index = (int) Stack.Pop();
                    int arrayReference = (int) Stack.Pop();
                    var heapObject = Heap.Get(arrayReference);
                    var array = (int[]) heapObject.Data;
                    Stack.Push(array[index]);
                    break;
                }
                case "ldloc.0": PushLocalToStack(0); break;
                case "ldloc.1": PushLocalToStack(1); break;
                case "ldloc.2": PushLocalToStack(2); break;
                case "ldloc.3": PushLocalToStack(3); break;
                case "ldloc.s": PushLocalToStack((byte) instruction.Operand); break;
                case "ldstr":
                {
                    var reference = Heap.Store(instruction.Operand, typeof(string));
                    Stack.Push(reference);
                    break;
                }
                case "mul":
                {
                    dynamic v2 = Stack.Pop();
                    dynamic v1 = Stack.Pop();
                    Stack.Push(v1 * v2);
                    break;
                }
                case "newarr":
                {
                    int size = (int)Stack.Pop();
                    var arrayType = (Type) instruction.Operand;
                    var array = Array.CreateInstance(arrayType, size);
                    var reference = Heap.Store(array, array.GetType());
                    Stack.Push(reference);
                    break;
                }
                case "nop": break;
                case "rem":
                {
                    dynamic v2 = Stack.Pop();
                    dynamic v1 = Stack.Pop();
                    Stack.Push(v1 % v2);
                    break;
                }
                case "stelem.i1":
                case "stelem.i2":
                case "stelem.i4":
                {
                    int value = (int) Stack.Pop();
                    int index = (int) Stack.Pop();
                    int arrayReference = (int) Stack.Pop();
                    var heapObject = Heap.Get(arrayReference);
                    var array = (int[]) heapObject.Data;
                    array[index] = value;
                    break;
                }
                case "stloc.0": PopFromStackToLocal(0); break;
                case "stloc.1": PopFromStackToLocal(1); break;
                case "stloc.2": PopFromStackToLocal(2); break;
                case "stloc.3": PopFromStackToLocal(3); break;
                case "stloc.s": PopFromStackToLocal((byte) instruction.Operand); break;
                case "sub":
                {
                    dynamic v2 = Stack.Pop();
                    dynamic v1 = Stack.Pop();
                    Stack.Push(v1 - v2);
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
                    var method = instruction.Operand as MethodInfo;
                    if (method == null) break;

                    var arguments = method.GetParameters().Select(param => Stack.Pop()).ToArray();
                    Array.Reverse(arguments);

                    object result = null;
                    if (method.IsStatic)
                    {
                        if (method.Module.Name == "mscorlib.dll")
                            result = method.Invoke(null, arguments);
                        else
                        {
                            var nestedInterpreter = new Interpreter(Heap);
                            result = nestedInterpreter.Run(method, method.ReturnType != typeof(void));
                        }
                    }
                    else
                    {
                        var targetReference = Stack.Pop();
                        var targetInstance = Heap.Get((int) targetReference);

                        if (method.Module.Name == "mscorlib.dll" || method.IsSpecialName)
                            result = method.Invoke(targetInstance.Data, arguments);
                        else
                        {
                            throw new NotImplementedException("this type of method call is not supported yet");
                        }
                    }

                    if (result != null)
                        Stack.Push(result);
                    break;
                }
                case "newobj":
                {
                    var ctor = instruction.Operand as ConstructorInfo;
                    if (ctor == null) break;

                    var parameters = ctor.GetParameters();
                    var arguments = new object[parameters.Length];
                    for (int i = parameters.Length - 1; i >= 0; i--)
                    {
                        var arg = Stack.Pop();
                        if (!parameters[i].ParameterType.IsValueType)
                        {
                            var heapObject = Heap.Get((int) arg);
                            arguments[i] = heapObject.Data;
                        }
                        else
                        {
                            arguments[i] = arg;
                        }
                    }

                    var newObject = ctor.Invoke(arguments);
                    var reference = Heap.Store(newObject, newObject.GetType());
                    Stack.Push(reference);
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
                    return (int) instruction.Operand;
                case "brfalse":
                case "brfalse.s":
                    if ((int) Stack.Pop() == 0)
                        return (int) instruction.Operand;

                    return -1;
                case "brtrue":
                case "brtrue.s":
                    if ((int) Stack.Pop() == 1)
                        return (int) instruction.Operand;
                    
                    return -1;
                default:
                    throw new NotImplementedException(instruction.Code.Name + " is not implemented.");
            }
        }

        private void PushLocalToStack(byte index)
        {
            Stack.Push(Locals[index]);
        }

        private void PopFromStackToLocal(byte index)
        {
            Locals[index] = Stack.Pop();
        }
    }
}