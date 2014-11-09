using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MsilInterpreterLib;

namespace MsilInterpreter.Tests
{
    [TestClass]
    public class InterpreterTests
    {
        private readonly Interpreter interpreter = new Interpreter();
        
        private void IfStatement()
        {
            int x = 10;
            int y = 0;

            if (x == 10)
                y = 20;

            int z = y;
        }

        [TestMethod]
        public void IfStatementTest()
        {
            interpreter.Run(IfStatement);
            Assert.IsTrue(interpreter.Stack.Count == 0);
            Assert.IsTrue((int) interpreter.Locals[0] == 10); // x
            Assert.IsTrue((int) interpreter.Locals[1] == 20); // y
            Assert.IsTrue((int) interpreter.Locals[2] == 20); // z
        }

        private void ElseStatement()
        {
            int x = 10;
            int y = 0;

            if (x == 20)
                y = 10;
            else
                y = 20;

            int z = y;
        }

        [TestMethod]
        public void ElseStatementTest()
        {
            interpreter.Run(ElseStatement);
            Assert.IsTrue(interpreter.Stack.Count == 0);
            Assert.IsTrue((int) interpreter.Locals[0] == 10); // x
            Assert.IsTrue((int) interpreter.Locals[1] == 20); // y
            Assert.IsTrue((int) interpreter.Locals[2] == 20); // z
        }

        private void CountingWithInt32()
        {
            int a = 10;
            int b = a + 20;
            int c = a - 5;
            int d = a * a;
        }

        [TestMethod]
        public void CountingWithInt32Test()
        {
            interpreter.Run(CountingWithInt32);
            Assert.IsTrue(interpreter.Stack.Count == 0);
            Assert.IsTrue((int) interpreter.Locals[0] == 10); // a
            Assert.IsTrue((int) interpreter.Locals[1] == 30); // b
            Assert.IsTrue((int) interpreter.Locals[2] == 5); // c
            Assert.IsTrue((int) interpreter.Locals[3] == 100); // d
        }

        private void ForLoop()
        {
            int x = 0;
            for (int i = 0; i < 10; i++)
            {
                x += i;
            }
        }

        [TestMethod]
        public void ForLoopTest()
        {
            interpreter.Run(ForLoop);
            Assert.IsTrue(interpreter.Stack.Count == 0);
            Assert.IsTrue((int) interpreter.Locals[0] == 45); // i
            Assert.IsTrue((int) interpreter.Locals[1] == 10); // x
        }

        private void StaticMethodCall()
        {
            double x = Math.Pow(10, 2);
        }

        [TestMethod]
        public void StaticMethodCallTest()
        {
            interpreter.Run(StaticMethodCall);
            Assert.IsTrue(interpreter.Stack.Count == 0);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Assert.IsTrue((double) interpreter.Locals[0] == 100);
        }

        private void StringHandling()
        {
            string text = "Hello, word!";
        }

        [TestMethod]
        public void StringHandlingTest()
        {
            interpreter.Run(StringHandling);
            Assert.IsTrue(interpreter.Stack.Count == 0);
            var heapString = interpreter.Heap.FirstGen.Values.FirstOrDefault();
            Assert.IsNotNull(heapString);
            Assert.AreEqual(heapString.Data, "Hello, word!");
        }
    }
}
