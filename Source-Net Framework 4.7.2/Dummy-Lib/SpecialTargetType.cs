using System;
using System.Collections.Generic;
using System.Text;
using dnlib.DotNet;

namespace DummyLib
{
    /// <summary>
    /// A Special <see cref="TargetType"/>
    /// </summary>
    public sealed class SpecialTargetType : TargetType
    {
        /// <summary>
        /// Target
        /// </summary>
        public TypeDef Target { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_type"></param>
        public SpecialTargetType(TypeDef _type) : base(_type)
        {
            Target = _type;
        }
    }
}
