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
    /// Creates Instruction With Pointers 
    /// </summary>
    public static class InstructionCreator
    {
        /// <summary>
        /// Creates a <see cref="InstructionPoint"/>
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="opperand"></param>
        /// <param name="replacementPoint">Where to place the <paramref name="opcode"/> and <paramref name="opperand"/></param>
        /// <returns></returns>
        public static InstructionPoint CreateInstruction(OpCode opcode, object opperand, int replacementPoint)
        {
            return new InstructionPoint(new Instruction(opcode, opperand), replacementPoint);
        }

        /// <summary>
        /// Creats an Empty <see cref="InstructionPoint"/>
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="replacementPoint">Where to place the <paramref name="opcode"/></param>
        /// <returns></returns>
        public static InstructionPoint CreateEmptyInstruction(OpCode opcode, int replacementPoint)
        {
            return new InstructionPoint(Instruction.Create(opcode), replacementPoint);
        }
    }
}
