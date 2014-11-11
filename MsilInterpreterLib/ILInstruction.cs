using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MsilInterpreterLib
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
                        fieldOp.FieldType.ToString(),
                        fieldOp.ReflectedType.ToString(),
                        fieldOp.Name);                                        
                case OperandType.InlineMethod:
                    var methodOp = Operand as MethodInfo;
                    if (methodOp != null)
                    {
                        return string.Format("{0} {1}{2} {3}::{4}()",
                            result,
                            !methodOp.IsStatic ? "instance " : "",
                            methodOp.ReturnType.ToString(),
                            methodOp.ReflectedType.ToString(),
                            methodOp.Name);
                    }
                    else
                    {
                        var ctorOp = Operand as ConstructorInfo;
                        return string.Format("{0} {1} {2}::{3}()",
                            result,
                            !ctorOp.IsStatic ? "instance " : "",
                            ctorOp.ReflectedType.ToString(),
                            ctorOp.Name);
                    }                    
                case OperandType.ShortInlineBrTarget:
                case OperandType.InlineBrTarget:
                    return string.Format("{0} {1:D4}", result, (int)Operand);                                        
                case OperandType.InlineType:
                    return result + " " + Operand.ToString();
                case OperandType.InlineString:
                    if (Operand.ToString() == "\r\n")
                        return result + " \"\\r\\n\"";
                    else
                        return result + " \"" + Operand + "\"";                    
                case OperandType.ShortInlineVar:
                    return result + Operand.ToString();
                case OperandType.InlineI:
                case OperandType.InlineI8:
                case OperandType.InlineR:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineR:
                    return result + " " + Operand.ToString();
                case OperandType.InlineTok:
                    return result + (Operand as Type)?.FullName ?? " not supported";
                default:
                    return result + " not supported";
            }
        }
    }
}