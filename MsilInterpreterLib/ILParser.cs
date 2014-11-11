using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

// list of op-codes: http://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes(v=vs.110).aspx

namespace MsilInterpreterLib
{
    internal sealed class ILParser
    {
        private readonly OpCode[] singleByteCodes = new OpCode[256];
        private readonly OpCode[] doubleByteCodes = new OpCode[256];

        private MethodInfo parsedMethodInfo;
        private byte[] ilBytes;
        private int ilBytesPosition;
        private readonly List<ILInstruction> instructions = new List<ILInstruction>();

        public ILParser()
        {
            LoadOpCodes();
        }

        public IEnumerable<ILInstruction> ParseILFromMethod(MethodInfo info)
        {
            parsedMethodInfo = info;
            instructions.Clear();
            
            if (parsedMethodInfo.GetMethodBody() != null)
                ParseInstructions();

            return instructions;
        }

        private void ParseInstructions()
        {
            ilBytes = parsedMethodInfo.GetMethodBody().GetILAsByteArray();
            ilBytesPosition = 0;

            while (ilBytesPosition < ilBytes.Length)
            {                
                OpCode code = OpCodes.Nop;
                int codePosition = ilBytesPosition;

                ushort value = ilBytes[ilBytesPosition++];                
                if (value != 0xFE)
                {
                    code = singleByteCodes[value];
                }
                else
                {
                    value = ilBytes[ilBytesPosition++];
                    code = doubleByteCodes[value];
                }

                var instruction = new ILInstruction(code, codePosition);
                instruction.Operand = ReadOperandData(instruction.Code.OperandType);
                instructions.Add(instruction);
            }
        }

        private object ReadOperandData(OperandType operandType)
        {
            var module = parsedMethodInfo.Module;

            switch (operandType)
            {
                case OperandType.InlineBrTarget:
                    return ReadInt32() + ilBytesPosition;
                case OperandType.InlineField:
                    return module.ResolveField(ReadInt32());
                case OperandType.InlineI:
                    return ReadInt32();
                case OperandType.InlineI8:
                    return ReadInt64();
                case OperandType.InlineMethod:
                    int metadataToken = ReadInt32();
                    try { return module.ResolveMethod(metadataToken); }
                    catch { return module.ResolveMember(metadataToken); }
                case OperandType.InlineNone:
                    return null;
                case OperandType.InlineR:
                    return ReadDouble();
                case OperandType.InlineSig:
                    return module.ResolveSignature(ReadInt32());
                case OperandType.InlineString:
                    return module.ResolveString(ReadInt32());
                case OperandType.InlineSwitch:
                    int count = ReadInt32();
                    var cases = new int[count];
                    for (int i = 0; i < count; i++)
                    {
                        cases[i] = ReadInt32() + ilBytesPosition;
                    }
                    return cases;
                case OperandType.InlineTok:
                    try { return module.ResolveType(ReadInt32()); }
                    catch { return null; }
                case OperandType.InlineType:
                    return module.ResolveType(ReadInt32(), parsedMethodInfo.DeclaringType.GetGenericArguments(), parsedMethodInfo.GetGenericArguments());
                case OperandType.InlineVar:
                    return ReadUInt16();
                case OperandType.ShortInlineBrTarget:
                    return ReadSByte() + ilBytesPosition; 
                case OperandType.ShortInlineI:
                    return ReadSByte();
                case OperandType.ShortInlineR:
                    return ReadSingle();
                case OperandType.ShortInlineVar:
                    return ReadByte();
                default:
                    throw new Exception("Unknown operand type.");
            }
        }

        #region IL read methods

        private sbyte ReadSByte()
        {
            return (sbyte) ilBytes[ilBytesPosition++];
        }

        private byte ReadByte()
        {
            return ilBytes[ilBytesPosition++];
        }

        private ushort ReadUInt16()
        {
            return (ushort) ((ilBytes[ilBytesPosition++] | (ilBytes[ilBytesPosition++] << 8)));
        }

        private int ReadInt32()
        {
            int result = BitConverter.ToInt32(ilBytes, ilBytesPosition);
            ilBytesPosition += 4;
            return result;
        }

        private long ReadInt64()
        {
            long result = BitConverter.ToInt64(ilBytes, ilBytesPosition);
            ilBytesPosition += 8;
            return result;
        }

        private float ReadSingle()
        {
            float result = BitConverter.ToSingle(ilBytes, ilBytesPosition);
            ilBytesPosition += 4;
            return result;
        }

        private double ReadDouble()
        {
            double result = BitConverter.ToDouble(ilBytes, ilBytesPosition);
            ilBytesPosition += 8;
            return result;
        }

        #endregion

        private void LoadOpCodes()
        {
            var opCodesFields = typeof(OpCodes).GetFields();
            foreach (var field in opCodesFields)
            {
                var fieldValue = (OpCode)field.GetValue(null);
                var instructionValue = (ushort)fieldValue.Value;

                if (instructionValue < 0x100)
                {
                    singleByteCodes[instructionValue] = fieldValue;
                }
                else
                {
                    doubleByteCodes[instructionValue & 0xFF] = fieldValue;
                }
            }
        }
    }
}