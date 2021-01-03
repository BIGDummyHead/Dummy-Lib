using dnlib.DotNet;
using DummyLib;
using System;

namespace DummyLibCodeExamples
{
    public class Example4
    {
        static void Main()
        {
            using (AssemblyWriter writer = new AssemblyWriter("Target.dll", "Output.dll"))
            {
                Resolver _resolver = new Resolver();

                //adds and resolves all inherited Types of TypeToAdd
                //ITypeResolver can be null
                //writer.AddExistingType(typeof(TypeToAdd)); if null it will use BasicTypeResolver
                writer.AddExistingType(typeof(TypeToAdd), _resolver);

                //writer.DisposeSave means that when the AssemblyWriter is disposed it will Save
                //we will only save if the Writer Can Write
                writer.DisposeSave = writer.CanWrite;
            }


        }
    }

    public class Resolver : BasicTypeResolver
    {
        //Called when a Type Is Resolved
        public override TypeDef ResolveType(AssemblyWriter writer, TypeData id, DummyLib.ITypeResolver resolver)
        {
            TypeDef _return = base.ResolveType(writer, id, resolver);

            if(_return != null)
            {
                Console.WriteLine($"{this.GetType().Name} Resolved The Type : {id.TypeName}");
            }

            return _return;
        }
    }

    public class CustomResolver : DummyLib.ITypeResolver
    {
        public TypeDef ResolveType(AssemblyWriter writer, TypeData id, DummyLib.ITypeResolver resolver)
        {
            //See how to resolve your type -> https://imgur.com/hsoBg7K
            throw new NotImplementedException("Not Implemented");
        }
    }

    public class TypeToAdd : Inheritable, IFace
    {
        public override void SayMessage(string message)
        {
            Console.WriteLine("User: ");
            base.SayMessage(message);
        }

        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    public interface IFace
    {
        int Add(int a, int b);
    }

    public class Inheritable
    {
        public virtual void SayMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
