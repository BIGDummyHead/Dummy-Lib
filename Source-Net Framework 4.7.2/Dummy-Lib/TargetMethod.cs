using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyLib
{
    /// <summary>
    /// A Method to target from a <see cref="Type"/>
    /// </summary>
    public class TargetMethod
    {
        /// <summary>
        /// The Method's Name
        /// </summary>
        public string MethodName { get; }
        /// <summary>
        /// The Method's Type Arguments -- Can Be Null
        /// </summary>
        public Type[] MethodArgs { get; } = null;

        /// <summary>
        /// The Method's TypeSig Arguments -- Never Null
        /// </summary>
        public TypeSig[] MethodArgs_Sig { get; } = null;

        /// <summary>
        /// The Method's Signature : Returns Null If Not Assigned 
        /// </summary>
        public MethodSig Method { get; }

        private ModuleContext _con = ModuleDef.CreateModuleContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="methodArguments"></param>
        public TargetMethod(string methodName, params Type[] methodArguments)
        {
            this.MethodName = methodName;
            this.MethodArgs = methodArguments;

            List<TypeSig> _sigs = new List<TypeSig>();


            foreach (Type _type in methodArguments)
            {
                ModuleDefMD _mod = ModuleDefMD.Load(_type.Module, _con);

                string loadName = AssemblyWriter.GetFullName(_type);

                TypeDef _found = _mod.Find(loadName, AssemblyWriter.IsReflectionName(loadName));

                _sigs.Add(_found.ToTypeSig());
            }

            this.MethodArgs_Sig = _sigs.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="methodArguments"></param>
        public TargetMethod(string methodName, TypeSig[] methodArguments)
        {
            this.MethodName = methodName;
            this.MethodArgs_Sig = methodArguments;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="methodSig"></param>
        /// <param name="methodArguments"></param>
        public TargetMethod(string methodName, MethodSig methodSig, TypeSig[] methodArguments)
        {
            this.MethodName = methodName;
            this.MethodArgs_Sig = methodArguments;
            this.Method = methodSig;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="methodSig"></param>
        /// <param name="methodArguments"></param>
        public TargetMethod(string methodName, MethodSig methodSig, params Type[] methodArguments)
        {
            this.MethodName = methodName;
            this.MethodArgs = methodArguments;
            this.Method = methodSig;

            List<TypeSig> _sigs = new List<TypeSig>();

            foreach (Type _type in methodArguments)
            {
                ModuleDefMD _mod = ModuleDefMD.Load(_type.Module, _con);

                string loadName = AssemblyWriter.GetFullName(_type);

                TypeDef _found = _mod.Find(loadName, AssemblyWriter.IsReflectionName(loadName));

                _sigs.Add(_found.ToTypeSig());
            }

            this.MethodArgs_Sig = _sigs.ToArray();
        }

        /// <summary>
        /// An Override Of <see cref="ToString"/> Provides a Beatuiful Display of Your Method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string _ret = $"Method Targeted \"{this.MethodName}\":  \nArguments:";
            _ret += "\n{";
            if (MethodArgs != null)
            {
                int i = 0;
                foreach (Type _t in MethodArgs)
                {
                    string name = $"{_t.Name}_{i}";

                    _ret += $"\n     {name}";

                    i++;
                }

                _ret += "\n}";
            }
            else
            {
                int i = 0;
                foreach (TypeSig _t in MethodArgs_Sig)
                {
                    string name = $"{_t.GetName()}_{i}";

                    _ret += $"\n     {name}";

                    i++;
                }

                _ret += "\n}";


            }

            return _ret;
        }


    }
}
