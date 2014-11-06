using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MsilInterpreterLib;

namespace MsilInterpreter.Tests
{
    [TestClass]
    public sealed class ILParserTests
    {
        private readonly ILParser parser = new ILParser();

        private void AssignNumbers()
        {
#pragma warning disable CS0219            
            sbyte b1 = -10;
            byte b2 = 200;
            short s1 = -10000;
            ushort s2 = 50000;
            int i1 = -100000;
            uint i2 = 3000000000;
            long l1 = -10000000000;
            ulong l2 = 10000000000000000000;
#pragma warning restore CS0219
        }

        public void AssignNumbersTest()
        {
            Action testMethod = AssignNumbers;
            var instructions = parser.ParseILFromMethod(testMethod.GetMethodInfo());
            const string expectedIL =
@"IL_0000: nop
IL_0001: ldc.i4.s -10
IL_0003: ldc.i4.s 200
IL_0005: ldc.i4 -10000
IL_0009: ldc.i4 50000
IL_000D: ldc.i4 -100000
IL_0011: ldc.i4 3000000000
IL_0015: ldc.i8 -10000000000
IL_001D: ldc.i8 10000000000000000000
IL_0025: ret";
            Assert.AreEqual(expectedIL, string.Join(Environment.NewLine, instructions));
        }

        private void AddIntsAndPrint()
        {
            int x = 10;
            int y = x + 20;
            Console.WriteLine(y);
        }

        [TestMethod]
        public void AddIntsAndPrintTest()
        {            
            Action testMethod = AddIntsAndPrint;
            var instructions = parser.ParseILFromMethod(testMethod.GetMethodInfo());
            const string expectedIL =
@"IL_0000: nop
IL_0001: ldc.i4.s 10
IL_0003: stloc.0
IL_0004: ldloc.0
IL_0005: ldc.i4.s 20
IL_0007: add
IL_0008: stloc.1
IL_0009: ldloc.1
IL_0010: call System.Void System.Console::WriteLine()
IL_0015: nop
IL_0016: ret";
            Assert.AreEqual(expectedIL, string.Join(Environment.NewLine, instructions));
        }
    }
}
