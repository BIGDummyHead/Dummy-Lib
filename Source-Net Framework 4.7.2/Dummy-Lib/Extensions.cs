using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using dnlib.DotNet.Emit;

namespace DummyLib
{
    /// <summary>
    /// Extensions For Dnlib Types and or DummyLib Types
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Takes a <see cref="ITypeDefOrRef"/> and makes a <see cref="TargetType"/>
        /// </summary>
        /// <param name="tRef"></param>
        /// <returns></returns>
        public static SpecialTargetType ToTarget(this ITypeDefOrRef tRef) => new SpecialTargetType(tRef.ScopeType.ResolveTypeDef());

        /// <summary>
        /// Returns a <see cref="SpecialTargetType"/> back to a <seealso cref="TypeDef"/>
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TypeDef ToType(this SpecialTargetType target) => target.Target;

        /// <summary>
        /// Gets A Method From A <see cref="IEnumerable{MethodDef}"/> From A <seealso cref="Type"/>[]
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MethodDef GetMethod(this IEnumerable<MethodDef> methods, string name, params Type[] args)
        {
            methods = methods.Where(x => x.Name == name);

            if (methods == null)
                return default;

            if (methods.Count() < 1)
                return default;

            foreach (MethodDef method in methods)
            {
                bool isMethod = true;

                if (method.MethodSig.Params.Count == args.Length)
                {
                    isMethod = true;
                    for (int i = 0; i < args.Length; i++)
                    {
                        Type current = args[i];
                        TypeSig currentSig = method.MethodSig.Params[i];

                        SigComparer compare = new SigComparer();

                        if (!compare.Equals(current, currentSig))
                            isMethod = false;
                    }

                    if (isMethod)
                    {
                        return method;
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// Gets A Method From A <see cref="IEnumerable{MethodDef}"/> From A <seealso cref="TypeSig"/>[]
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MethodDef GetMethod(this IEnumerable<MethodDef> methods, string name, TypeSig[] args)
        {
            methods = methods.Where(x => x.Name == name);

            if (methods == null)
                return default;

            if (methods.Count() < 1)
                return default;


            foreach (MethodDef method in methods)
            {
                bool isMethod = true;

                if (method.MethodSig.Params.Count == args.Length)
                {
                    isMethod = true;
                    for (int i = 0; i < args.Length; i++)
                    {
                        TypeSig current = args[i];
                        TypeSig currentSig = method.MethodSig.Params[i];

                        if (current != currentSig)
                            isMethod = false;
                    }

                    if (isMethod)
                    {
                        return method;
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// Get a method from a TypeDef providing a <see cref="string"/> <paramref name="name"/> and providing <seealso cref="Type"/>[] <paramref name="args"/>
        /// </summary>
        /// <param name="receiveMethods"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static MethodDef GetMethod(this TypeDef receiveMethods, string name, params Type[] args) => receiveMethods.Methods.GetMethod(name, args);

        /// <summary>
        /// Get a method from a TypeDef providing a <see cref="string"/> <paramref name="name"/> and providing <seealso cref="TypeSig"/>[] <paramref name="args"/>
        /// </summary>
        /// <param name="receiveMethods"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static MethodDef GetMethod(this TypeDef receiveMethods, string name, TypeSig[] args) => receiveMethods.Methods.GetMethod(name, args);

        /// <summary>
        /// Takes a <see cref="MethodDef"/> and makes a <seealso cref="SpecialTargetMethod"/>
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static SpecialTargetMethod ToTarget(this MethodDef method)
        {
            return new SpecialTargetMethod(method);
        }

        /// <summary>
        /// Takes a <see cref="SpecialTargetMethod"/> and makes a <seealso cref="MethodDef"/>
        /// </summary>
        /// <param name="specialMethod"></param>
        /// <returns></returns>
        public static MethodDef ToMethod(this SpecialTargetMethod specialMethod) => specialMethod.Target;

        private static ModuleContext backingContext = ModuleDef.CreateModuleContext();

        /// <summary>
        /// The main context of <see cref="CreateModule(Module)"/>
        /// </summary>
        public static ModuleContext MainContext => backingContext;


        /// <summary>
        /// Creates A Module From See <see cref="Module"/>
        /// <para>Mainly Used As <seealso cref="Type"/>.<seealso cref="Module"/>.<seealso cref="CreateModule(Module)"/></para>
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static ModuleDefMD CreateModule(this Module module)
        {
            return ModuleDefMD.Load(module, MainContext);
        }

        /// <summary>
        /// Easily Find A Type
        /// </summary>
        /// <param name="module"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static TypeDef Find(this ModuleDefMD module, string fullName)
        {
            return module.Find(fullName, AssemblyWriter.IsReflectionName(fullName));
        }

        /// <summary>
        /// Create an <see cref="InstructionPoint"/> From a OpCode
        /// </summary>
        /// <param name="op"></param>
        /// <param name="opperand"></param>
        /// <param name="replacementPoint"></param>
        /// <returns></returns>
        public static InstructionPoint CreateInstruction(this OpCode op, object opperand, int replacementPoint)
        {
            return InstructionCreator.CreateInstruction(op, opperand, replacementPoint);
        }

        /// <summary>
        /// Create an Empty <see cref="InstructionPoint"/>
        /// </summary>
        /// <param name="op"></param>
        /// <param name="replacementPoint"></param>
        /// <returns></returns>
        public static InstructionPoint CreateEmptyInstruction(this OpCode op, int replacementPoint)
        {
            return InstructionCreator.CreateEmptyInstruction(op, replacementPoint);
        }
    }
}
