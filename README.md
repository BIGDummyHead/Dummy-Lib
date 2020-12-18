#Dummy-Lib

Dummy Lib is a library built off of dnlib, this was made to make adding methods, fields, properties, and instructions easy
I plan on working on Constructor and Type adding in the future

##Opening an AssemblyWriter

To open an Assembly Writer it is pretty simple
There are around 6 AssemblyWriter CTORs but I will just use the longest one so you can get an understanding

    //                     Should the assemblywriter throw its internal errors?  Should the Writer backup the targetfile?
    //AssemblyWriter(string targetFile, string output, bool throwInternalErrors, bool backUpFile)
    AssemblyWriter writer = new AssemblyWriter("Target.dll", "dllToWriteTo.dll", true, true);
    
##Creating a Method 

    //open a writer
    AssemblyWriter writer = new AssemblyWriter("target.dll");
    
    //find a TargetType in the target.dll
    TargetType typeTarg = new TargetType("Namespace.TypeName");
    
We also need a class that inherits IMethodCreater so let's do that first

    public class MethodToInsert : IMethodCreater
    {
        //The interface requires you to add a Type[] property, this array should include the types of your methods arguments as follows
        public Type[] MethodArgs { get { return new Type[] { typeof(int), typeof(int) }; } }
        
        //now the interface does not require you to have a method but the AssemblyWriter.CreateMethod does
        //your methods name should be named "Method" and that is about it for requirements
        //I will now make a method that adds
        
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
    
    //make a usermethod with a name and IMethodCreater
    UserMethod yourMethod = new UserMethod(new MethodToInsert(), "AddNums");
    
    //let's add our method
    CreatedMethod method = writer.CreateMethod(typeTarg, yourMethod);
    
    //we can then save our method to the output by using the save method
    writer.Save();
    

##Creating A Field
    
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
    
 ##Creating A Property
 
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
    
    
===============================================================================================


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
    

##Creating Instructions In A Method

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

    
    
    
