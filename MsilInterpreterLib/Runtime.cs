using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MsilInterpreterLib.Components;
using MsilInterpreterLib.Framework;
using MsilInterpreterLib.Msil;

namespace MsilInterpreterLib
{
    public sealed class Runtime
    {
        private readonly ILParser ilParser = new ILParser();
        private readonly List<DotAssembly> loadedAssemblies = new List<DotAssembly>();
        private readonly Heap heap = new Heap(100, 1000);
        private readonly Stack<StackFrame> callStack = new Stack<StackFrame>();

        /// <summary>
        /// All loaded assemblies which store all type system metadata (Method Tables).
        /// </summary>
        internal ReadOnlyCollection<DotAssembly> LoadedAssemblies { get { return loadedAssemblies.AsReadOnly(); } }
        internal Heap Heap { get { return heap; } }
        internal Stack<StackFrame> CallStack { get { return callStack; } }

        public Runtime()
        {
            LoadFramework();
        }

        public void LoadAssembly(string path)
        {
            var refAssembly = Assembly.LoadFrom(path);
            var dotAssembly = new DotAssembly(refAssembly.GetName().Name, false);

            foreach (var refType in refAssembly.GetTypes())
            {
                var dotType = new DotType(refType.Name, dotAssembly);
                dotType.Fields = FindFields(refType, dotType);
                dotType.Constructors = FindConstructors(refType, dotType);
                dotType.Methods = FindMethods(refType, dotType);
                dotAssembly.Types.Add(dotType);
            }

            loadedAssemblies.Add(dotAssembly);
        }

        public void StartExecution(string[] args)
        {
            var entryPoint = loadedAssemblies.SelectMany(a => a.Types)
                                             .SelectMany(t => t.Methods)
                                             .FirstOrDefault(m => m.Name == "Main");
            if (entryPoint == null)
                throw new InvalidOperationException("No loaded assembly contains Main method representing an entry point.");

            var interpreter = new Interpreter(this);
            var input = interpreter.CreateRefTypeArray(args);
            var initStackFrame = new StackFrame(null, entryPoint);
            initStackFrame.Arguments = new List<object> { input };
            CallStack.Push(initStackFrame);
            interpreter.Execute(entryPoint);
        }

        private List<DotField> FindFields(Type type, DotType inType)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)
                       .Select(f => new DotField(f.Name, inType))
                       .ToList();
        }

        private List<DotConstructor> FindConstructors(Type type, DotType inType)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)
                       .Select(m => new DotConstructor
                        (
                           inType, 
                           m.IsStatic, 
                           m.GetParameters().Select(p => p.ParameterType).ToArray(), 
                           ilParser.ParseILFromMethod(m).ToList())
                        ).ToList();
        }

        private List<DotMethod> FindMethods(Type type, DotType inType)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)
                       .Select(m => new DotMethod
                        (
                           m.Name, 
                           inType, 
                           m.IsStatic, 
                           m.ReturnType, 
                           m.GetParameters().Select(p => p.ParameterType).ToArray(), 
                           ilParser.ParseILFromMethod(m).ToList())
                           {
                               IsAbstract = m.IsAbstract,
                               IsVirtual = m.IsVirtual
                           }
                        ).ToList();
        }

        /// <summary>
        /// Initialize types from mscorlib that are used in the chosen problem.
        /// </summary>
        private void LoadFramework()
        {
            var fwAssembly = new DotAssembly("mscorlib", true);

            var objectType = new DotType("Object", fwAssembly);
            objectType.Constructors.Add(new ObjectCtor(objectType));
            objectType.Methods.Add(new ObjectToString(objectType));
            fwAssembly.Types.Add(objectType);

            var stringType = new DotType("String", fwAssembly);
            stringType.Fields.Add(new DotField("Value", stringType));
            stringType.Methods = new List<DotMethod>
            {
                new StringConcat3(stringType),
                new StringFormat(stringType),
                new StringJoin(stringType),
                new StringSplit(stringType)
            };
            fwAssembly.Types.Add(stringType);

            var intType = new DotType("Int32", fwAssembly);
            intType.Methods.Add(new Int32Parse(intType));
            fwAssembly.Types.Add(intType);

            var arrayType = new DotType("Array", fwAssembly);
            arrayType.Fields = new List<DotField>
            {
                new DotField("Values", arrayType),
                new DotField("Length", arrayType)
            };
            fwAssembly.Types.Add(arrayType);

            var listType = new DotType("List`1", fwAssembly);
            listType.Fields.Add(new DotField("Values", listType));
            listType.Constructors.Add(new ListCapacityCtor(listType));
            listType.Methods = new List<DotMethod>
            {
                new ListAddRange(listType),
                new ListGetCount(listType),
                new ListGetItem(listType),
                new ListGetRange(listType),
                new ListIndexOf(listType),
                new ListInsert(listType),
                new ListRemoveAt(listType),
                new ListRemoveRange(listType)
            };
            fwAssembly.Types.Add(listType);

            var fileType = new DotType("File", fwAssembly);
            fileType.Methods = new List<DotMethod>
            {
                new FileReadAllLines(fileType),
                new FileWriteAllText(fileType)
            };
            fwAssembly.Types.Add(fileType);

            loadedAssemblies.Add(fwAssembly);
        }
    }
}