using System;
using DummyLib;

namespace DummyLibCodeExamples
{
    class Example3
    {
        static void Main()
        {
            using (AssemblyWriter writer = new AssemblyWriter("Target.dll", "Output.dll"))
            {
                //get a target
                TargetType targetT = new TargetType("Namespace.Name");

                //this UserType specifies the type will be created as such
                //Type : DummyLibCodeExamples.TestType -> Empty Type under the namespace with the same name and uses the same type Attributes
                UserType userT = new UserType(typeof(TestType));

                //Creates -> an Empty type under -> MyExamples -> with the name Example -> Has The Attributes of Public
                UserType _type = new UserType("MyExamples", "Example", dnlib.DotNet.TypeAttributes.Public);

                //you can use the attributes of an existing type if you that better suites your needs
                UserType _TEST_TYPE_ = new UserType("ExampleNamespace", "ExampleName", (dnlib.DotNet.TypeAttributes)typeof(TestType).Attributes);

                //the TypeDef created by the writer
                dnlib.DotNet.TypeDef newType = writer.CreateType(userT);

                //adds the existing type of "TestType" with the method Add
                dnlib.DotNet.TypeDef _typeGenerated = writer.AddExistingType(typeof(TestType));

                //The AddExistingType method uses a Type called "ExistingType"
                //let's take a look
                ExistingType _exist = new ExistingType(typeof(TestType));

                //this provides us will all sorts of things!
                //_exist.Attributes;
                //_exist.Events;
                //_exist.Properties;
                //_exist.Methods;
                //_exist.FullName;
                //This inherits the Type "UserType" I figured I show you this because it is a neat thing to use!

                //save
                writer.Save();
            }
        }
    }

    public class TestType 
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
