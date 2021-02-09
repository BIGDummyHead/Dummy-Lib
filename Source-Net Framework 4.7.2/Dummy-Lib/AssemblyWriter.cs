using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace DummyLib
{

    public sealed partial class AssemblyWriter
    {

        private List<string> _changes = new List<string>();

        /// <summary>
        /// Changes :: Provides Actions You Did
        /// </summary>
        public IReadOnlyList<string> Changes => _changes;

        private void AddChange(string change)
        {
            _changes.Add($"{change}_{_changes.Count}");
        }
    }

    //contains other extensions and method adding   

    /// <summary>
    /// Deals with writing to DLL and Executables Files
    /// </summary>
    public sealed partial class AssemblyWriter : IDisposable, ICloneable
    {
        /// <summary>
        /// Creates Method(s)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="methods"></param>
        /// <returns></returns>
        public CreatedMethod[] CreateMethods(TargetType target, IMethodCreator methods)
        {

            List<CreatedMethod> created = new List<CreatedMethod>();

            foreach (KeyValuePair<string, Type[]> method in methods.Methods)
            {
                ModuleDefMD module = Module;

                TypeDef def = module.Find(target.TargetName, target.IsReflectionName);

                if (ThrowException(def == null, "Could Not Find The Type You Were Looking For"))
                    return default;

                Type methodType = methods.GetType();



                ModuleDefMD methodModule = ModuleDefMD.Load(methodType.Module, _context);
                TypeDef methodTypedef = methodModule.Find(methodType.FullName, !methodType.FullName.Contains("+"));

                MethodDef foundMethod = methodTypedef.Methods.GetMethod(method.Key, method.Value);

                if (ThrowException(foundMethod == null, "Could Not Find Your Method You Want To Override"))
                    return default;

                if (ThrowException(def.FindMethod(method.Key, foundMethod.MethodSig) != null, $"This Method Already Exist In {def.Name}"))
                    return default;


                bool exist = def.Methods.GetMethod(method.Key, method.Value) != null;

                if (ThrowException(exist, "This Method Already Exist"))
                    return default;


                foundMethod.DeclaringType = null;

                MethodDef methodToInsert = foundMethod;

                methodToInsert.Attributes = foundMethod.Attributes;

                methodToInsert.Body = foundMethod.Body;

                methodToInsert.Name = method.Key;

                methodToInsert.ReturnType = foundMethod.ReturnType;

                def.Methods.Add(methodToInsert);

                CreatedMethod _CMeth = new CreatedMethod(new TargetMethod(method.Key, methodToInsert.MethodSig, method.Value), methodToInsert);

                created.Add(_CMeth);
            }

            AddChange($"User Created Methods({created.Count})");

            return created.ToArray();
        }

        #region Context and ModuleDef
        private ModuleContext _context = ModuleDef.CreateModuleContext();

        /// <summary>
        /// The context of the Module
        /// </summary>
        public ModuleContext Context => _context;

        /// <summary>
        /// The module you are working on
        /// </summary>
        public ModuleDefMD Module { get; private set; }
        #endregion



        #region Extensions

        /// <summary>
        /// Returns <see cref="TargetMethod"/>[]
        /// <para>Returns Specifically <seealso cref="TargetMethod.TargetMethod(string, TypeSig[])"/></para>
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public TargetMethod[] GetMethods(TargetType target)
        {
            TypeDef td = Module.Find(target.TargetName, target.IsReflectionName);

            if (ThrowException(td is null, "Type You Wanted To Find Is Null"))
                return default;

            List<TargetMethod> _methods = new List<TargetMethod>();
            foreach (MethodDef method in td.Methods)
            {
                TargetMethod tm = new TargetMethod(method.Name, method.MethodSig.Params.ToArray());
                _methods.Add(tm);
            }

            return _methods.ToArray();
        }

        /// <summary>
        /// Get all the types from the <see cref="Module"/> and returns them as <seealso cref="TargetType"/>
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<TargetType> GetTypes()
        {
            List<TargetType> targets = new List<TargetType>();

            foreach (TypeDef def in Module.Types)
            {
                string name = GetFullName(def);
                TargetType tar = new TargetType(name);
                targets.Add(tar);
            }

            return targets;
        }

        /// <summary>
        /// Find a Type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TypeDef FindType(string name) => this.Module?.Find(name, IsReflectionName(name));

        #endregion

    }

    //deals with Constructors and other properties does not contain the context and module def
    public sealed partial class AssemblyWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="output"></param>
        /// <param name="throwInternalErrors"></param>
        /// <param name="backUpFile"></param>
        public AssemblyWriter(string targetFile, string output, bool throwInternalErrors, bool backUpFile)
        {
            this.TargetFile = targetFile;
            this.Output = output;
            this.ThrowWriterErrors = throwInternalErrors;

            if (backUpFile)
                CreateBackupFile(targetFile);

            Module = ModuleDefMD.Load(File.ReadAllBytes(targetFile));
        }

        /// <summary>
        /// The file to edit
        /// </summary>
        public string TargetFile { get; }
        /// <summary>
        /// Should the <see cref="AssemblyWriter"/> Throw internal method errors 
        /// </summary>
        public bool ThrowWriterErrors { get; set; }
        /// <summary>
        /// Where the file will be written to
        /// </summary>
        public string Output { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetfile"></param>
        public AssemblyWriter(string targetfile)
        {
            TargetFile = targetfile;
            Output = targetfile;
            ThrowWriterErrors = false;
            Module = ModuleDefMD.Load(File.ReadAllBytes(Output));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="outputFile"></param>
        public AssemblyWriter(string targetFile, string outputFile)
        {
            this.TargetFile = targetFile;
            this.Output = outputFile;
            ThrowWriterErrors = false;
            Module = ModuleDefMD.Load(File.ReadAllBytes(targetFile));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="throwInternalErrors"></param>
        public AssemblyWriter(string targetFile, string outputFile, bool throwInternalErrors)
        {
            this.TargetFile = targetFile;
            this.Output = outputFile;
            ThrowWriterErrors = throwInternalErrors;
            Module = ModuleDefMD.Load(File.ReadAllBytes(targetFile));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="throwInternalErrors"></param>
        public AssemblyWriter(string targetFile, bool throwInternalErrors)
        {
            this.TargetFile = targetFile;
            this.Output = targetFile;
            this.ThrowWriterErrors = throwInternalErrors;
            Module = ModuleDefMD.Load(File.ReadAllBytes(Output));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="throwInternalErrors"></param>
        /// <param name="backUpFile"></param>
        public AssemblyWriter(string targetFile, bool throwInternalErrors, bool backUpFile)
        {
            this.TargetFile = targetFile;
            this.Output = targetFile;
            this.ThrowWriterErrors = throwInternalErrors;

            if (backUpFile)
                CreateBackupFile(targetFile);

            Module = ModuleDefMD.Load(File.ReadAllBytes(Output));

        }
    }

    //handles saving and other things -- CanSave and more
    public sealed partial class AssemblyWriter
    {
        /// <summary>
        /// Checks if a Type Name is a Reflected Type
        ///<para>Reflection : Namespace.Name</para>
        ///<para>Nested : Namespace.Name+NestedName</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsReflectionName(string name) => !name.Contains("+");

        /// <summary>
        /// Checks if a Type Name is a Nested Type
        ///<para>Reflection : Namespace.Name</para>
        ///<para>Nested : Namespace.Name+NestedName</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsNestedName(string name) => !IsReflectionName(name);

        /// <summary>
        /// Get the full name of a <see cref="TypeDef"/>
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetFullName(TypeDef def)
        {
            string name = def.Name;
            string _nS = def.Namespace;
            string ret = string.Empty;
            if (string.IsNullOrEmpty(_nS))
                ret = name;
            else
                ret = $"{_nS}.{name}";

            return ret;
        }

        /// <summary>
        /// Get the full name of a <see cref="Type"/>
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string GetFullName(Type def)
        {
            string name = def.Name;
            string _nS = def.Namespace;
            string ret = string.Empty;
            if (string.IsNullOrEmpty(_nS))
                ret = name;
            else
                ret = $"{_nS}.{name}";

            return ret;
        }


        /// <summary>
        /// Clones the Assembly Writer
        /// Does not create backup file...
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            AssemblyWriter toCop = new AssemblyWriter(this.TargetFile, this.Output, this.ThrowWriterErrors, false);
            toCop.DisposeSave = this.DisposeSave;
            return toCop;
        }

        private void CreateBackupFile(string file)
        {
            string fileBackName = $"{Path.GetFileName(file).Split('.').First()}{new Random().Next(0, 100)}.bak";

            if (!Directory.Exists("Backups"))
                Directory.CreateDirectory("Backups");

            string pathToFile = $"{Directory.GetCurrentDirectory()}\\Backups\\{fileBackName}";

            if (File.Exists(pathToFile))
            {
                fileBackName = $"{Path.GetFileName(file)}{new Random().Next(101, 200)}.bak";
                pathToFile = $"{Directory.GetCurrentDirectory()}\\Backups\\{fileBackName}";
            }

            File.Copy(file, pathToFile);
        }

        /// <summary>
        /// If <paramref name="isBad"/> is True and <see cref="ThrowWriterErrors"/> is True it will throw an exception
        /// <para>If <paramref name="isBad"/> is True and <seealso cref="ThrowWriterErrors"/> is False it will return True</para>
        /// </summary>
        /// <param name="isBad"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ThrowException(bool isBad, string message)
        {
            if (isBad)
                if (ThrowWriterErrors)
                    throw new Exception(message);
                else
                    return true;

            return false;

        }


        /// <summary>
        /// Saves all your work to the <see cref="Output"/>
        /// </summary>
        public void Save()
        {
            Module.Write(Output);
        }

        /// <summary>
        /// Dispose this <see cref="AssemblyWriter"/>
        /// </summary>
        public void Dispose()
        {
            if (DisposeSave)
                this.Save();

            this.Module.Dispose();
            this.Module = null;
            this._context = null;
        }

        /// <summary>
        /// Saves the Module <see cref="Save"/> -> and <seealso cref="Dispose"/>
        /// </summary>
        public void SaveAndDispose()
        {
            bool change = false;

            this.Save();
            if (DisposeSave)
            {
                DisposeSave = false;
                change = true;
            }
            this.Dispose();

            if (change)
                DisposeSave = true;
        }

        /// <summary>
        /// Does not acutally save to your current <see cref="Output"/> but sees if you can save to your <seealso cref="Output"/>
        /// </summary>
        /// <returns></returns>
        public bool CanSave(ref string reason)
        {
            string testing = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\TESTOUTPUT.dll";

            try
            {
                File.Create(testing).Close();

                Module.Write(testing);

                File.Delete(testing);

                reason = null;

                return true;
            }
            catch (Exception ex)
            {
                if (File.Exists(testing))
                    File.Delete(testing);

                reason = ex.Message;
            }

            return false;
        }

        private string userHiddenReason;

        /// <summary>
        /// Same as <see cref="CanSave(ref string)"/> but does not include reasoning
        /// </summary>
        public bool CanWrite { get => CanSave(ref userHiddenReason); }

        /// <summary>
        /// Save on Dispose
        /// </summary>
        public bool DisposeSave { get; set; } = false;

        /// <summary>
        /// WARNING: TAKES EXTRA LONG TO SAVE BUT ENSURES THE MODULE WRITER DOES NOT THROW ERRORS
        /// </summary>
        /// <returns></returns>
        public bool TrySave()
        {
            bool can = CanWrite;
            if (can)
                Module.Write(Output);

            return can;
        }

    }

    //deals with type resolution and adding different types
    public sealed partial class AssemblyWriter
    {
        /// <summary>
        /// Copies the constructors from <paramref name="typeCTORS"/> and paste them to <paramref name="target"/>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="typeCTORS"></param>
        public MethodDef[] CopyConstructors(TargetType target, IHasConstructors typeCTORS) => Hide_CopyConstructors(target, typeCTORS).ToArray();

        //Copies the constructors from a type that implements the IHasConstructors interface
        private IEnumerable<MethodDef> Hide_CopyConstructors(TargetType target, IHasConstructors typeCtors)
        {
            Type t = typeCtors.GetType();

            ModuleDefMD md = ModuleDefMD.Load(t.Module, _context);

            string full = GetFullName(t);

            TypeDef td = md.Find(full, !full.Contains("+"));

            List<MethodDef> ctors = td.Methods.Where(x => x.IsConstructor).ToList();

            TypeDef toAdd = Module.Find(target.TargetName, !target.TargetName.Contains("+"));

            int z = 0;
            for (int i = 0; i < ctors.Count; i++)
            {
                MethodDef cur = ctors[i];

                cur.DeclaringType = null;
                toAdd.Methods.Add(cur);

                z++;
                yield return cur;
            }

            AddChange($"User Added Constructors({z})");
        }

        /// <summary>
        /// Creates a Type To The <see cref="Module"/>
        /// </summary>
        /// <param name="userType">A User Type</param>
        public TypeDef CreateType(UserType userType)
        {
            if (ThrowException(Module.Find(userType.FullName, !userType.FullName.Contains("+")) != null, "This Type Already Exist"))
                return default;

            TypeDefUser userT = new TypeDefUser(userType.Namespace, userType.Name);
            userT.Attributes = userType.Attributes;
            Module.Types.Add(userT);

            AddChange($"User Created The Type :: {userT.FullName}");

            return Module.Find(userType.FullName, !userType.FullName.Contains("+"));
        }

        /// <summary>
        /// Add an existing <see cref="Type"/> to the <seealso cref="Module"/> with a different namespace and name
        /// <para>Does not work with Inheritance</para>
        /// </summary>
        /// <param name="existingType"></param>
        /// <param name="newNameSpace"></param>
        /// <param name="name"></param>
        /// <param name="resolver">If not supplied, the Writer will supply one for you <see cref="BasicTypeResolver"/></param>
        /// <returns></returns>
        public TypeDef AddExistingType(Type existingType, string newNameSpace, string name, ITypeResolver resolver = null)
        {

            #region Start
            ExistingType exs = new ExistingType(existingType, newNameSpace, name);

            TypeDef dd = Module.Find(exs.FullName, !exs.FullName.Contains("+"));

            if (dd != null)
            {
                Module.Types.Remove(dd);
            }

            if (resolver == null)
                resolver = new BasicTypeResolver();

            TypeDefUser userT = new TypeDefUser(exs.Namespace, exs.Name);
            #endregion

            #region Resolve
            {
                for (int e = 0; e < exs.Interfaces.Count(); e++)
                {
                    InterfaceImpl _i = exs.Interfaces.ToArray()[e];

                    ITypeDefOrRef reffed = null;

                    TypeDef importedInterface = _i.Interface.ScopeType.ResolveTypeDef();

                    ResolveData dat = new ResolveData(importedInterface);

                    if (dat.ImportType != null)
                        reffed = resolver.ResolveType(this, dat, newNameSpace, resolver);

                    InterfaceImpl myInterface = new InterfaceImplUser(reffed);

                    if (reffed != default || reffed != null)
                    {
                        userT.Interfaces.Add(myInterface);
                    }
                }

                ResolveData iD = new ResolveData(exs.Inherit);
                ITypeDefOrRef resolvedInherit = null;
                if (iD.ImportType != null)
                    resolvedInherit = resolver.ResolveType(this, iD, newNameSpace, resolver);

                if (resolvedInherit != null)
                    userT.BaseType = resolvedInherit;
            }
            #endregion

            #region Main
            userT.Attributes = exs.Attributes;

            Module.Types.Add(userT);

            TypeDef created = Module.Find(userT.FullName, !userT.FullName.Contains("+"));

            foreach (FieldDef m in exs.Fields)
            {
                m.DeclaringType = null;
                created.Fields.Add(m);
            }

            Dictionary<string, MethodDef> specMethods = new Dictionary<string, MethodDef>();
            List<MethodDef> __IMPORTANT_CTOR_defs = new List<MethodDef>();

            if (exs.Inherit != null)
            {
                List<MethodDef> meths = exs.Inherit?.ResolveTypeDef()?.Methods.ToList();

                if (meths != null)
                    for (int i = 0; i < meths.Count(); i++)
                    {
                        if (meths[i].IsConstructor)
                        {
                            meths[i].DeclaringType = null;
                            __IMPORTANT_CTOR_defs.Add(meths[i]);
                        }
                    }
            }

            foreach (MethodDef m in exs.Methods)
            {
                if (!m.IsSpecialName || m.IsConstructor)
                {
                    m.DeclaringType = null;
                    created.Methods.Add(m);


                }
                else if (m.IsSpecialName)
                {
                    m.DeclaringType = null;

                    specMethods.Add(m.Name, m);
                }
            }

            foreach (MethodDef m in __IMPORTANT_CTOR_defs)
            {
                created.Methods.Add(m);
            }

            foreach (EventDef m in exs.Events)
            {
                m.DeclaringType = null;
                created.Events.Add(m);
            }

            foreach (PropertyDef def in exs.Properties)
            {
                MethodAttributes attrs;

                TypeSig sig = def.PropertySig.RetType;

                if (def.GetMethod is null)
                {
                    attrs = MethodAttributes.Public;
                }
                else
                    attrs = def.GetMethod.Attributes;


                def.DeclaringType = null;
                PropertyDef newProp = Property<int>.CreateEmptyProperty(this, new TargetType(created), def.Name, attrs);
                newProp.PropertySig.RetType = sig;

                created.Methods.Remove(newProp.GetMethod);
                created.Methods.Remove(newProp.SetMethod);

                if (def.GetMethod != null)
                {
                    MethodDef getter = specMethods[def.GetMethod.Name];

                    created.Methods.Add(getter);
                    newProp.GetMethod = getter;
                    newProp.GetMethod.ReturnType = getter.ReturnType;
                    newProp.GetMethod.Name = getter.Name;
                }
                else
                {
                    newProp.GetMethod = null;
                    newProp.SetMethod = null;
                }

                if (def.SetMethod != null)
                {
                    MethodDef setter = specMethods[def.SetMethod.Name];

                    created.Methods.Add(setter);
                    newProp.SetMethod = setter;
                    newProp.SetMethod.Name = setter.Name;
                }
                else
                    newProp.SetMethod = null;
            }

            AddChange($"User Added The The Type :: {created.FullName}");

            #endregion
            return created;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingType"></param>
        /// <param name="resolver"></param>
        /// <param name="newNamespace"></param>
        /// <returns></returns>
        public TypeDef AddExistingType(ITypeDefOrRef existingType, string newNamespace, ITypeResolver resolver = null)
        {
            #region Start
            if (resolver == null)
                resolver = new BasicTypeResolver();

            ExistingType exs = new ExistingType(this, existingType, resolver);

            TypeDef dd = Module.Find(exs.FullName, !exs.FullName.Contains("+"));

            if (dd != null)
            {
                Module.Types.Remove(dd);
            }

            TypeDefUser userT = new TypeDefUser(exs.Namespace, exs.Name);
            #endregion

            #region Resolve
            {
                for (int e = 0; e < exs.Interfaces.Count(); e++)
                {
                    InterfaceImpl _i = exs.Interfaces.ToArray()[e];

                    ITypeDefOrRef reffed = null;

                    TypeDef importedInterface = _i.Interface.ScopeType.ResolveTypeDef();

                    ResolveData dat = new ResolveData(importedInterface);

                    if (dat.ImportType != null)
                        reffed = resolver.ResolveType(this, dat, newNamespace, resolver);

                    InterfaceImpl myInterface = new InterfaceImplUser(reffed);

                    if (reffed != default || reffed != null)
                        userT.Interfaces.Add(myInterface);
                }

                ResolveData iD = new ResolveData(exs.Inherit);
                ITypeDefOrRef resolvedInherit = null;
                if (iD.ImportType != null)
                    resolvedInherit = resolver.ResolveType(this, iD, newNamespace, resolver);

                if (resolvedInherit != null)
                    userT.BaseType = resolvedInherit;
            }
            #endregion

            #region Main
            userT.Attributes = exs.Attributes;

            Module.Types.Add(userT);

            TypeDef created = Module.Find(userT.FullName, !userT.FullName.Contains("+"));
            created.Namespace = newNamespace;

            foreach (FieldDef m in exs.Fields)
            {
                m.DeclaringType = null;
                created.Fields.Add(m);
            }

            Dictionary<string, MethodDef> specMethods = new Dictionary<string, MethodDef>();
            List<MethodDef> __IMPORTANT_CTOR_defs = new List<MethodDef>();

            if (exs.Inherit != null)
            {
                List<MethodDef> meths = exs.Inherit?.ResolveTypeDef()?.Methods.ToList();

                if (meths != null)
                    for (int i = 0; i < meths.Count(); i++)
                    {
                        if (meths[i].IsConstructor)
                        {
                            meths[i].DeclaringType = null;
                            __IMPORTANT_CTOR_defs.Add(meths[i]);
                        }
                    }
            }

            foreach (MethodDef m in exs.Methods)
            {
                if (!m.IsSpecialName || m.IsConstructor)
                {
                    m.DeclaringType = null;
                    created.Methods.Add(m);


                }
                else if (m.IsSpecialName)
                {
                    m.DeclaringType = null;

                    specMethods.Add(m.Name, m);
                }
            }

            foreach (MethodDef m in __IMPORTANT_CTOR_defs)
            {
                created.Methods.Add(m);
            }

            foreach (EventDef m in exs.Events)
            {
                m.DeclaringType = null;
                created.Events.Add(m);
            }

            foreach (PropertyDef def in exs.Properties)
            {
                MethodAttributes attrs;

                TypeSig sig = def.PropertySig.RetType;

                if (def.GetMethod is null)
                {
                    attrs = MethodAttributes.Public;
                }
                else
                    attrs = def.GetMethod.Attributes;


                def.DeclaringType = null;
                PropertyDef newProp = Property<int>.CreateEmptyProperty(this, new TargetType(created), def.Name, attrs);
                newProp.PropertySig.RetType = sig;

                created.Methods.Remove(newProp.GetMethod);
                created.Methods.Remove(newProp.SetMethod);

                if (def.GetMethod != null)
                {
                    MethodDef getter = specMethods[def.GetMethod.Name];

                    created.Methods.Add(getter);
                    newProp.GetMethod = getter;
                    newProp.GetMethod.ReturnType = getter.ReturnType;
                    newProp.GetMethod.Name = getter.Name;
                }
                else
                {
                    newProp.GetMethod = null;
                    newProp.SetMethod = null;
                }

                if (def.SetMethod != null)
                {
                    MethodDef setter = specMethods[def.SetMethod.Name];

                    created.Methods.Add(setter);
                    newProp.SetMethod = setter;
                    newProp.SetMethod.Name = setter.Name;
                }
                else
                    newProp.SetMethod = null;
            }
            #endregion
            AddChange($"User Added The The Type :: {created.FullName}");

            return created;

        }

        /// <summary>
        /// Add an Existing Type with the same namespace as the <paramref name="existingType"/>
        /// </summary>
        /// <param name="existingType"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public TypeDef AddExistingType(ITypeDefOrRef existingType, ITypeResolver resolver = null) => AddExistingType(existingType, existingType.Namespace, resolver);

        /// <summary>
        /// Adds an existing <see cref="Type"/> to the <seealso cref="Module"/>
        /// </summary>
        /// <param name="existingType"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public TypeDef AddExistingType(Type existingType, ITypeResolver resolver = null)
        {
            return AddExistingType(existingType, existingType.Namespace, existingType.Name, resolver);
        }
    }

    //deals with instruction adding etc...
    public sealed partial class AssemblyWriter
    {
        /// <summary>
        /// Adds an Instruction To A Method...
        /// <para>This method has a chance of throwing many errors if you do not know where you should place your Instruction</para>
        /// <para>Also see: <see cref="InstructionCreator.CreateInstruction(OpCode, object, int)"/></para>
        /// </summary>
        public CilBody AddInstruction(TargetType target, TargetMethod method, InstructionPoint instruction)
        {
            ModuleDefMD mod = Module;
            TypeDef tDef = mod.Find(target.TargetName, target.IsReflectionName);

            IEnumerable<MethodDef> foundMethods = tDef.Methods.Where(x => x.Name == method.MethodName);

            if (ThrowException(foundMethods.Count() < 1, "No Methods Found"))
                return default;

            MethodDef methodFound = null;

            if (foundMethods.Count() == 1)
                methodFound = foundMethods.FirstOrDefault();
            else if (foundMethods.Count() > 1 && method.MethodArgs != null)
                methodFound = tDef.Methods.GetMethod(method.MethodName, method.MethodArgs);
            else if (foundMethods.Count() > 1 && method.MethodArgs_Sig != null)
                methodFound = tDef.Methods.GetMethod(method.MethodName, method.MethodArgs_Sig);

            if (ThrowException(methodFound is null, "Could Not Find Method"))
                return default;

            List<Instruction> newIns = new List<Instruction>();

            for (int i = 0; i < methodFound.Body.Instructions.Count; i++)
            {
                if (i == instruction.ReplacementPoint)
                {
                    newIns.Add(instruction.Instruction);
                    newIns.Add(methodFound.Body.Instructions[i]);
                }
                else
                {
                    newIns.Add(methodFound.Body.Instructions[i]);
                }

            }

            methodFound.Body.Instructions.Clear();

            foreach (Instruction i in newIns)
                methodFound.Body.Instructions.Add(i);

            AddChange($"User Added An Instruction :: Method : {methodFound.Name} : Instruction : {instruction}");

            return methodFound.Body;
        }

        /// <summary>
        /// Removes An Instruction From A Method...
        /// <para>This method has a chance of throwing many errors if you do not know where you should remvove your Instruction</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="removalPoint"></param>
        /// <returns></returns>
        public CilBody RemoveInstruction(TargetType target, TargetMethod method, int removalPoint)
        {
            ModuleDefMD mod = Module;
            TypeDef tDef = mod.Find(target.TargetName, target.IsReflectionName);

            IEnumerable<MethodDef> foundMethods = tDef.Methods.Where(x => x.Name == method.MethodName);

            if (ThrowException(foundMethods.Count() < 1, "No Methods Found"))
                return default;

            MethodDef methodFound = null;

            if (foundMethods.Count() == 1)
                methodFound = foundMethods.FirstOrDefault();
            else if (foundMethods.Count() > 1 && method.MethodArgs != null)
                methodFound = tDef.Methods.GetMethod(method.MethodName, method.MethodArgs);
            else if (foundMethods.Count() > 1 && method.MethodArgs_Sig != null)
                methodFound = tDef.Methods.GetMethod(method.MethodName, method.MethodArgs_Sig);

            if (ThrowException(methodFound is null, "Could Not Find Method"))
                return default;

            methodFound.Body.Instructions.RemoveAt(removalPoint);

            AddChange($"User Removed An Instruction :: Method : {methodFound.Name} : At : {removalPoint}");

            return methodFound.Body;
        }

        /// <summary>
        /// Adds multiple Instructions To A Method
        /// <para>This method has a chance of throwing many errors if you do not know where you should add your Instructions</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="instructions"></param>
        /// <returns></returns>

        public CilBody[] AddInstructions(TargetType target, TargetMethod method, InstructionPoint[] instructions)
        {
            List<CilBody> bodies = new List<CilBody>();
            foreach (InstructionPoint ins in instructions)
                bodies.Add(AddInstruction(target, method, ins));

            return bodies.ToArray();
        }

        /// <summary>
        /// Removes Multiple Instructions From A Method...
        /// <para>This method has a chance of throwing many errors if you do not know where you should remvove your Instructions</para>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="removalPoints"></param>
        /// <returns></returns>
        public CilBody[] RemoveInstructions(TargetType target, TargetMethod method, int[] removalPoints)
        {
            List<CilBody> bodies = new List<CilBody>();

            foreach (int i in removalPoints)
                bodies.Add(RemoveInstruction(target, method, i));

            return bodies.ToArray();
        }
    }

    //field and property adding
    public sealed partial class AssemblyWriter
    {
        /// <summary>
        /// Creates a Field In The <paramref name="target"/> Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="userField"></param>
        /// <returns></returns>
        public FieldDef CreateField<T>(TargetType target, UserField<T> userField)
        {
            ModuleDefMD tCurrent = ModuleDefMD.Load(typeof(T).Module, _context);
            TypeDef fieldAdder = Module.Find(target.TargetName, target.IsReflectionName);

            bool fieldExist = fieldAdder.FindField(userField.Field.FieldName) != null;

            if (ThrowException(fieldExist, "Field Exist Already"))
                return default;

            Type a = userField.Field.GetType();

            ModuleDefMD aMod = ModuleDefMD.Load(a.Module, _context);

            TypeDef tUse = aMod.Find(GetFullName(a), !GetFullName(a).Contains("+"));

            FieldDef fieldUser = tUse.FindField("Val");
            fieldUser.DeclaringType = null;

            fieldUser.Name = userField.Field.FieldName;
            fieldUser.Attributes = userField.Attributes;
            fieldUser.FieldType = tCurrent.Find(typeof(T).FullName, !typeof(T).FullName.Contains("+")).ToTypeSig();

            object val = userField.Value;

            fieldUser.Constant = new ConstantUser(val);
            fieldUser.Constant.Value = userField.Value;


            fieldAdder.Fields.Add(fieldUser);

            AddChange($"User Added A Field {fieldUser.Name}");

            return fieldUser;
        }

        /// <summary>
        /// Creates a property inside the <paramref name="target"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="userProp"></param>
        /// <param name="attrs"></param>
        public PropertyDef CreateProperty<T>(TargetType target, Property<T> userProp, MethodAttributes attrs)
        {
            TypeDef def = Module.Find(target.TargetName, target.IsReflectionName);

            if (ThrowException(def.FindProperty(userProp.Name) != null, "Property Already Exist!"))
                return default;

            ModuleDefMD p = ModuleDefMD.Load(userProp.GetType().Module, _context);

            TypeDef methodStore = p.Find(GetFullName(userProp.GetType()));

            MethodDef get = methodStore.FindMethod(nameof(userProp.GetMethod));
            MethodDef set = methodStore.FindMethod(nameof(userProp.SetMethod));
            PropertyDef old = methodStore.ScopeType.ResolveTypeDef()?.FindProperty("Val");
           
            if (ThrowException(old == null, "Could Not Resolve Type"))
                return default;

            bool setterEmpty = set.Body.Instructions[0].OpCode == OpCodes.Nop && set.Body.Instructions[1].Operand?.ToString() == "EMPTY" && set.Body.Instructions[2].Operand?.ToString() == "System.Void System.NotImplementedException::.ctor(System.String)" && set.Body.Instructions[3].OpCode == OpCodes.Throw;
            bool getterEmpty = get.Body.Instructions[0].OpCode == OpCodes.Nop && get.Body.Instructions[1].Operand?.ToString() == "EMPTY" && get.Body.Instructions[2].Operand?.ToString() == "System.Void System.NotImplementedException::.ctor(System.String)" && get.Body.Instructions[3].OpCode == OpCodes.Throw;

            //throw here to save some stuff
            if (ThrowException(getterEmpty && !setterEmpty, "Your Get Method Cannot Be Empty While Your Set Method Is Not"))
                return default;

            old.DeclaringType = null;
            set.DeclaringType = null;
            get.DeclaringType = null;

            PropertyDef made = old;

            made.Name = userProp.Name;

            string getOldName = get.Name;
            get.Name = $"get_{getOldName}";


            string setOldName = set.Name;
            set.Name = $"set_{getOldName}";

            set.Attributes = attrs;
            get.Attributes = attrs;

            def.Methods.Add(get);
            made.GetMethod = get;

            def.Methods.Add(set);
            made.SetMethod = set;

            if (getterEmpty && setterEmpty)
            {
                made.GetMethod.Body = null;
                made.SetMethod.Body = null;
            }

            if (!getterEmpty && setterEmpty)
            {
                made.SetMethod = null;
                def.Methods.Remove(set);
            }

            TypeSig sigT = ModuleDefMD.Load(typeof(T).Module, _context).Find(GetFullName(typeof(T)), !typeof(T).FullName.Contains("+")).ToTypeSig();

            made.PropertySig.RetType = sigT;
            made.GetMethod.ReturnType = sigT;
            def.Properties.Add(made);

            AddChange($"User Added Property : {made.Name}");

            return Module.Find(target.TargetName, target.IsReflectionName).FindProperty(userProp.Name);
        }
    }

}
