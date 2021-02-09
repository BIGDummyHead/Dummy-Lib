using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;

namespace DummyLib
{
    /// <summary>
    /// An abstract class, this class is meant to generate properties inside of your <see cref="TargetType"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Property<T>
    {
        /// <summary>
        /// Creates an Empty Property Of Type <typeparamref name="T"/>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="target"></param>
        /// <param name="propName">Name of said Property</param>
        /// <param name="propAttrs">Property Attributes</param>
        public static PropertyDef CreateEmptyProperty(AssemblyWriter writer, TargetType target, string propName, MethodAttributes propAttrs)
        {
            return writer.CreateProperty(target, new EmptyProperty<T>(propName), propAttrs);
        }

        /// <summary>
        /// Leave Me Empty
        /// </summary>
        public abstract T Val { get; set; }

        /// <summary>
        /// The method that is called when the property is Gotten
        /// <para>You can call throw <see cref="NotImplementedException"/>("EMPTY"); and the Get method will be empty</para>
        /// </summary>
        /// <returns></returns>
        public abstract T GetMethod();

        /// <summary>
        /// The method that is called when the property is Set
        /// <para>You can call throw <see cref="NotImplementedException"/>("EMPTY"); and the Get method will be empty</para>
        /// </summary>
        /// <returns></returns>
        public abstract void SetMethod();

        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Property(string name)
        {
            this.Name = name;
        }
    }

    /// <summary>
    /// An Empty Property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EmptyProperty<T> : Property<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public EmptyProperty(string name) : base(name)
        {
        }

        /// <summary>
        /// EMPTY
        /// </summary>
        public override T Val { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override T GetMethod()
        {
            throw new NotImplementedException("EMPTY");
        }
        /// <summary>
        /// 
        /// </summary>
        public override void SetMethod()
        {
            throw new NotImplementedException("EMPTY");
        }
    }
}
