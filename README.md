# Dummy-Lib

Dummy Lib is a library built off of dnlib, this was made to make adding methods, fields, properties, and instructions easy
I plan on working on Constructor and Type adding in the future

## Opening an AssemblyWriter

To open an Assembly Writer it is pretty simple
There are around 6 AssemblyWriter CTORs but I will just use the longest one so you can get an understanding

    //                     Should the assemblywriter throw its internal errors?  Should the Writer backup the targetfile?
    //AssemblyWriter(string targetFile, string output, bool throwInternalErrors, bool backUpFile)
    AssemblyWriter writer = new AssemblyWriter("Target.dll", "dllToWriteTo.dll", true, true);
    
__________________________________

## Creating a Method 

    //open a writer
    AssemblyWriter writer = new AssemblyWriter("target.dll");
    
    //find a TargetType in the target.dll
    TargetType typeTarg = new TargetType("Namespace.TypeName");
    
We also need a class that inherits MethodCreator so let's do that first

    public class MethodToInsert : MethodCreator
    {
        //The interface requires you to add a Type[] property, this array should include the types of your methods arguments as follows
        public Type[] MethodArgs { get { return new Type[] { typeof(int), typeof(int) }; } }
        
        //This override is the name of your method, the method's name will remain as "Method" until overriden
        public override string Name { get => "Add"; set => base.Name = value; }
        
        //now the abstract class does not require you to have a method but the AssemblyWriter.CreateMethod does
        //your methods name should be named after the overriden name you provide and that is about it for requirements
        //I will now make a method that adds two numbers together 
        
        //(Note: any attribute you add to this method will be applied to the method created)
        public static int Add(int a, int b)
        {
            return a + b;
        }
    }
    
Let's head back to our main code
    
    //open a writer
    AssemblyWriter writer = new AssemblyWriter("target.dll");
    
    //find a TargetType in the target.dll
    TargetType typeTarg = new TargetType("Namespace.TypeName");
    
    //make a usermethod with a name and MethodCreator
    UserMethod yourMethod = new UserMethod(new MethodToInsert(), "AddNums");
    
    //let's add our method
    CreatedMethod method = writer.CreateMethod(typeTarg, yourMethod);
    
    //we can then save our method to the output by using the save method
    writer.Save();
    
__________________________________

## Creating A Field
    
Creating a field is pretty simple so let's do it

    //open a writer 
    AssemblyWriter writer = new AssemblyWriter("target.dll");
    
    //find a TargetType in the target.dll
    TargetType typeTarg = new TargetType("Namespace.TypeName");
    
    //make a field
    UserField<string> uField = new UserField<string>("MyFieldName", "Value", FieldAttributes.Public);
    
    //A Field Is generated inside of the typeTarg
    FieldDef newField = writer.CreateField<string>(typeTarg, ufield);
    
    //save to the target
    writer.Save();
    
__________________________________
    
## Creating A Property
 
Creating a property is a little more difficult so let me show you how to do it

      //First make a class that inherits the Property<T>
      
      public class MyProp : Property<string>
      {
          public MyProp(string name) : base(name) {}
          
          //this will be based off of your T 
          public override string Val { get; set; }
          
          //this will be called everytime your property is got
          public override string GetMethod()
          {
              //code goes here
              return "Val";
          }
          
          //this will be called everytime your property is set
          public override void SetMethod()
          {
             //code goes here
          }
          
          /*
          what if we dont want to return a value?
          or we want our get and set methods to be empty?
          
          just do this!
          
          public override GetMethod()
          {
             throw NewNotImplementedException("EMPTY");
          }
          
          public override SetMethod()
          {
             throw NewNotImplementedException("EMPTY");
          }
          
          //generates public T NameHere { get; set; }
          */
      }
    
    
__________________________________


      //now let's write the property
      
      //open the writer
      AssemblyWriter writer = new AssemblyWriter("target.dll");
      
      //define the Target type
      TargetType typeTarg = new TargetType("Namespace.TypeName");
      
      //define our method attributes
      MethodAttributes methodAttrs = MethodAttributes.Public | MethodAttributes.Static;
      
      //create the property
      PropertyDef propDef = writer.CreateProperty<string>(typeTarg, new MyProp("PropName"), methodAttrs);
      
      //write to the target
      writer.Save();
    
__________________________________

## Creating Instructions In A Method

I will now show you how to add instructions to a method

            //Open the writer
            AssemblyWriter writer = new AssemblyWriter("Target.dll");

            //Find the type
            TargetType typeTarg = new TargetType("Namespace.Type");

            //Find the method with the name and the types that the arguments are
            TargetMethod method = new TargetMethod("MethodName", new Type[] { });

            //create a Instruction Point to be used in the AddInstruction method
            InstructionPoint inPoint = InstructionCreater.CreateInstruction(OpCodes.Call, null, 10);

            //remove the instruction from the method at index 10
            writer.RemoveInstruction(typeTarg, method, 10);

            //add the instruction to the method
            writer.AddInstruction(typeTarg, method, inPoint);

            //save to the output
            writer.Save();
            
            //the writer contains another method for adding and removing instructions, 
            //this allows you to add multiple instructions or even remove multiple instructions at the same time
__________________________________        

## Creating A Type

This is pretty simple and I made it this way to avoid confusion

            //open a writer
            AssemblyWriter writer = new AssemblyWriter("Target.dll");
            
            //we can either copy an existing type or create our own type in it's own namespace and have it's own name!
            //to create it is simple
            
            UserType userT = new UserType("Namespace", "Name", TypeAttributes);
            TypeDef tDef = writer.CreateType(userT);
            
            //you have successfully created a type and all you need to do is save your Assembly -> writer.Save() | writer.SaveAndDispose()
            //optionally doing 
            UserType userT = new UserType(typeof(ExistingType));
            
            //this will take the name of the already existing type and apply its same Attributes to it
            TypeDef tdef = writer.CreateType(userT);
            
            writer.Save();
    
__________________________________        

## Adding An Existing Type

This is even simpiler than creating your own Type

            //open a writer
            AssemblyWriter writer = new AssemblyWriter("Target.dll");
            
            //Does not support inheritance of other classes support coming VERY soon
            TypeDef tDef = writer.AddExistingType(typeof(YourExistingType));
            
            //save to file
            writer.Save();
__________________________________        

## Adding Constructors To A TargetType

            //Create a class with the interface IHasConstructors
            
            public class CTORHolder : IHasConstructors
            {
                public CTORHolder(string main)
                {
                
                }
                
                public CTORHolder(int i)
                {
                
                }
            }
            
            //open a writer
            AssemblyWriter writer = new AssemblyWriter("Target.dll");
            
            TargetType tar = new TargetType("Namespace.Name");
            
            //just make a new instance of CTORHolder doesnt matter which one
            IHasConstructors hasCTORS = new CTORHolder(0);
            
            //Copied!
            writer.CopyConstructors(tar, hasCTORS);
            
            //save to dll
            writer.Save();
            
__________________________________     

            
## Changes

AssemblyWriter now Inherits IDisposable --> using(AssemblyWriter writer = new AssemblyWriter("Target.dll")) -> Disposes
AssemblyWriter also allows you to save and then dispose at the same time! -> writer.SaveAndDispose();

AssemblyWriter also includes two new static methods -> AssemblyWriter.GetFullName(Type t); AssemblyWriter.GetFullName(TypeDef tDef);

+UserType Class
+ExistingType Class
+IHasConstructors Interface

__________________________________     

public sealed class EmptyProperty<T> : Property<T> -> Provides an empty property
Property<T> contains a new method named "CreateEmptyProperty" -> Property<string>.CreateEmptyProperty(AssemblyWriter writer, TargetType target, string propName, MethodAttributes attrs); -> Returns and Creates A PropertyDef inside of the TargetType

__________________________________   

## Coming Soon | 

I will soon add support for Existing Type with inheritance it will look something like this

            public class A 
            {
            
            }
            
            public class B : A, IA 
            {
            
            }
            
            public interface IA
            {
            
            }
            
            AssemblyWriter writer = new AssemblyWriter("Target.dll");
            
            writer.AddExistingType(typeof(B)); //this will make a loop that will keep adding types, to support said inheritance, so adding the Type B will also add Type A and the interface IA
            
            writer.SaveAndDispose();

More To Come!
