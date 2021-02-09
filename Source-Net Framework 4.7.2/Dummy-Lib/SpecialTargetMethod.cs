using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DummyLib
{
    /// <summary>
    /// A Special <see cref="TargetMethod"/>
    /// </summary>
    public sealed class SpecialTargetMethod : TargetMethod
    {
        /// <summary>
        /// The Target
        /// </summary>
        public MethodDef Target { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public SpecialTargetMethod(MethodDef method) : base(method.Name, method.MethodSig, method.MethodSig.Params.ToArray())
        {
            this.Target = method;
        }
    }
}
