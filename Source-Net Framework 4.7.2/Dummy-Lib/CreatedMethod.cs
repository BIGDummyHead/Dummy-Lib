using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyLib
{
    /// <summary>
    /// </summary>
    public sealed class CreatedMethod
    {
        /// <summary>
        /// The <see cref="TargetMethod"/> generated 
        /// </summary>
        public TargetMethod Target { get;  }

        /// <summary>
        /// The <see cref="MethodDef"/> generated
        /// </summary>
        public MethodDef MethodCreated { get;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="created"></param>
        public CreatedMethod(TargetMethod target, MethodDef created)
        {
            this.Target = target;
            this.MethodCreated = created;
        }
    }
}
