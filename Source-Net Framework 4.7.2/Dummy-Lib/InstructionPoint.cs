using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
namespace DummyLib
{
    /// <summary>
    /// Used in <see cref="DummyLib.AssemblyWriter"/>
    /// </summary>
    public sealed class InstructionPoint
    {
        /// <summary>
        /// The instruction that is created
        /// </summary>
        public Instruction Instruction { get; }

        /// <summary>
        /// Where the instruction should be placed
        /// </summary>
        public int ReplacementPoint { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="replacementPoint"></param>
        public InstructionPoint(Instruction instruction, int replacementPoint)
        {
            this.Instruction = instruction;
            this.ReplacementPoint = replacementPoint;
        }

        /// <summary>
        /// A Overview Of Your <see cref="InstructionPoint"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string ret = string.Empty;

            if (this.Instruction.Operand == null)
                return $"Point : {ReplacementPoint} \nOpCode : {this.Instruction.OpCode}";
            else
                return $"Point : {ReplacementPoint} \nOpCode : {this.Instruction.OpCode} \nOperand : {this.Instruction.Operand}";
        }
    }
}
