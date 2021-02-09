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
    
    //implement the IMethodCreator Interface
    public class MethodsToAdd : IMethodCreator
    {
    	//initialize Dictionary
    	public Dictionary<string, Type[]> Methods => new Dictionary<string, Type[]>()
        {
	    //Add our entrys
            { nameof(Add), new Type[] { typeof(int), typeof(int) } },
            { nameof(Sub), new Type[] { typeof(int), typeof(int) } },
            { nameof(Divide), new Type[] { typeof(int), typeof(int) } },
            { nameof(Multiply), new Type[] { typeof(int), typeof(int) } },
        };

	//methods to add
        public int Add(int a, int b) => a + b;
        public int Sub(int a, int b) => a - b;
        public int Divide(int a, int b) => a / b;
        public int Multiply(int a, int b) => a * b;
    }
    
    
    string targetDLL = @"Target.dll";

    //IDisposable :)
    using (AssemblyWriter writer = new AssemblyWriter(targetDLL))
    {
    	//find and make Target
        TargetType _gameStart = writer.FindType("GameStart").ToTarget();
		
	//our methods
        MethodsToAdd _methods = new MethodsToAdd();
		
	//add methods
        CreatedMethod[] _createdMethods = writer.CreateMethods(_gameStart, _methods);
	
	string error = string.Empty;
	
	bool _save = writer.CanSave(ref error);
	
	if(_save)
	{
	   writer.Save();
	}
	else
	{
	   Console.WriteLine($"Cannot Save Because: {error}");	
	}
                
    }
    
    
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
            
            //Supports Inheritance, adds all types that are inherited from your class you want to add
	    /*
		public class A : B, IMain {}
		public class B : C {}
		public class C {}
		public interface IMain {}
	    */
            TypeDef tDef = writer.AddExistingType(typeof(A));
	    //Adds all following Types -> C -> B -> IMain -> A | In this order
            
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

+Added Types

+UserType (Class)
+ExistingType (Class)
+IHasConstructors (Interface)
+IMethodCreator (Interface)

-Removed Types

-IMultipleMethodCreator (Interface)
-MethodCreator (Class)

+Added To AssemblyWriter

+ CanSave(ref string)
+ CanWrite => CanSave(ref string) => Hidden string
+ DisposeSave => Save On Dispose



Removed adding Single methods, seemed like a waste.

+Extensions

+ ToTarget(this ITypeDefOrRef tRef) => TargetType
+ ToTarget(this this MethodDef method) => TargetMethod
+ GetMethod(this IEnumerable<MethodDef> methods, string name, params Type[] args) => MethodDef
+ GetMethod(this IEnumerable<MethodDef> methods, string name, TypeSig[] args) => MethodDef
	
+Provided Overrides for ToString on TargetType and TargetMethod

+ Added a new method to AssemblyWriter
+ Find(string typeName)
+ (static) IsReflectionName(string name) => bool
+ (static) IsNestedName(string name) => bool => opposes IsReflectionName(string)

+AssemblyWriter Implements ICloneable => AssemblyWriter.Clone() => Object => AssemblyWriter myNewWriter = currentWriter.Clone() as AssemblyWriter => Cloned

__________________________________     

public sealed class EmptyProperty<T> : Property<T> -> Provides an empty property
Property<T> contains a new method named "CreateEmptyProperty" -> Property<string>.CreateEmptyProperty(AssemblyWriter writer, TargetType target, string propName, MethodAttributes attrs); -> Returns and Creates A PropertyDef inside of the TargetType

__________________________________   

## Coming Soon 

Attribute Support

More To Come!
