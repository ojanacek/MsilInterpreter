using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MsilInterpreterLib.Msil
{
    internal sealed class ILInstruction
    {
        private readonly OpCode code;
        private readonly int offset;

        public OpCode Code { get { return code; } }
        public int Offset { get { return offset; } }
        public object Operand { get; set; }

        public ILInstruction(OpCode code, int offset)
        {
            this.code = code;
            this.offset = offset;
        }

        public override string ToString()
        {
            var result = string.Format("IL_{0:D4}: {1}", Offset, Code);
            if (Operand == null) return result;

            switch (Code.OperandType)
            {
                case OperandType.InlineField:
                    var fieldOp = (FieldInfo) Operand;
                    return string.Format("{0} {1} {2}::{3}",
                        result,
                        fieldOp.FieldType,
                        fieldOp.ReflectedType,
                        fieldOp.Name);                                        
                case OperandType.InlineMethod:
                    var methodOp = Operand as MethodInfo;
                    if (methodOp != null)
                    {
                        return string.Format("{0} {1}{2} {3}.{4}({5})",
                            result,
                            !methodOp.IsStatic ? "instance " : "",
                            methodOp.ReturnType,
                            methodOp.ReflectedType,
                            methodOp.Name,
                            string.Join(",", methodOp.GetParameters().Select(p => p.ParameterType)));
                    }
                    var ctorOp = Operand as ConstructorInfo;
                    return string.Format("{0} {1} {2}.{3}({4})",
                        result,
                        !ctorOp.IsStatic ? "instance " : "",
                        ctorOp.ReflectedType,
                        ctorOp.Name,
                        string.Join(",", ctorOp.GetParameters().Select(p => p.ParameterType)));
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                    return string.Format("{0} {1:D4}", result, (int)Operand);                                        
                case OperandType.InlineType:
                    return result + " " + Operand;
                case OperandType.InlineString:
                    if (Operand.ToString() == "\r\n")
                        return result + " \"\\r\\n\"";
                    return result + " \"" + Operand + "\"";
                case OperandType.ShortInlineVar:
                    return result + Operand;
                case OperandType.InlineI:
                case OperandType.InlineI8:
                case OperandType.InlineR:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineR:
                    return result + " " + Operand;
                case OperandType.InlineTok:
                    var type = Operand as Type;
                    return result + (type == null ? " not supported" : type.FullName);
                default:
                    return result + " not supported";
            }
        }
    }
}