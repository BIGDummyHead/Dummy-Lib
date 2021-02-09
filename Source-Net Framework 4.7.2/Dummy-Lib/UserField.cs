using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyLib
{
    /// <summary>
    /// Generates a User Field
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UserField<T>
    {
        /// <summary>
        /// The Field that gets generated
        /// </summary>
        public Field<T> Field { get; }

        /// <summary>
        /// The initial value of the Field
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// The Attributes of the <see cref="Field"/>
        /// </summary>
        public FieldAttributes Attributes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="attributes"></param>
        public UserField(Field<T> field, T value, FieldAttributes attributes)
        {
            this.Field = field;
            this.Value = value;
            this.Attributes = attributes;
        }
        /// <summary>
        /// Make a <see cref="Field{T}"/> By <paramref name="fieldName"/>
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="attributes"></param>
        public UserField(string fieldName, T value, FieldAttributes attributes)
        {
            this.Field = new Field<T>(fieldName);
            this.Value = value;
            this.Attributes = attributes;
        }
    }
}
