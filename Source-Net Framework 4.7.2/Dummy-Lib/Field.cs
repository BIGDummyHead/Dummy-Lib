using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyLib
{
    /// <summary>
    /// A Field For <see cref="DummyLib.AssemblyWriter"/> (<typeparamref name="T"/>)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Field<T>
    {
        /// <summary>
        /// 
        /// </summary>
        internal T Val;

        /// <summary>
        /// The name of the field
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        public Field(string fieldName)
        {
            this.FieldName = fieldName;
        }
    }
}
