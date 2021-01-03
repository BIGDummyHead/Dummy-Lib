using System;
using DummyLib;

namespace DummyLibCodeExamples
{
    class Example
    {
        static void Main()
        {
            using(AssemblyWriter writer = new AssemblyWriter("Target.dll", "Output.dll"))
            {
                //get a target
                TargetType targetT = new TargetType("Namespace.Name");

                //create a usermethod, uses the MethodCreator.Name if you use this CTOR; This will the name of the method placed into the TargetType!
                UserMethod newMethod = new UserMethod(new Method());

                //created a method!
                CreatedMethod methodCreated = writer.CreateMethod(targetT, newMethod);

                //finds a method inside of a TypeTarget(defined later) by it's name and the types that are used in it, this can also be done with TypeDefs
                TargetMethod toAddInstructions = new TargetMethod("MethodName", new Type[] { typeof(string) });

                //create an instruction, the OpCode which will be a Call, and the opperand it is calling which is the MethodDef from the created method
                InstructionPoint iPoint = InstructionCreater.CreateInstruction(dnlib.DotNet.Emit.OpCodes.Call, methodCreated.MethodCreated, 0);

                //add the instruction
                writer.AddInstruction(targetT, toAddInstructions, iPoint);

                //save
                writer.Save();
            }
        }
    }

    public class Method : IMethodCreator
    {
        public override Type[] MethodArgs { get { return new Type[] { }; } }

        public override string Name { get => nameof(SayHelloWorld); set => base.Name = value; }

        public Dictionary<string, Type[]> Methods => new Dictionary<string, Type[]>()
        {
            { nameof(SayHelloWorld), new Type[] { } },
            { nameof(Say), new Type[] { typeof(string) } }
        };

        public void SayHelloWorld()
        {
            Console.WriteLine("Hello World");
        }

        public void Say(string name)
        {
            Console.WriteLine(name);
        }
    }
}
