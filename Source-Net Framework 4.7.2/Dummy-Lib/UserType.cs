using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace DummyLib
{
    /// <summary>
    /// </summary>
    public class UserType
    {
        /// <summary>
        /// The namespace of your user type
        /// </summary>
        public string Namespace { get;  }

        /// <summary>
        /// The name of the type
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The Attributes of The Type
        /// </summary>
        public TypeAttributes Attributes { get; }

        /// <summary>
        /// Returns The Full Name Of The User Type
        /// </summary>
        public string FullName
        {
            get
            {
                string main = string.Empty;
                string nS = this.Namespace;
                string name = this.Name;

                if (string.IsNullOrEmpty(nS))
                    main = name;
                else
                    main = $"{nS}.{name}";

                return main;
            }
        }

        /// <summary>
        /// Makes A Data Container For <see cref="AssemblyWriter.CreateType(UserType)"/>
        /// </summary>
        /// <param name="_namespace"></param>
        /// <param name="name"></param>
        /// <param name="attrs"></param>
        public UserType(string _namespace, string name, TypeAttributes attrs)
        {
            this.Namespace = _namespace;
            this.Name = name;
            this.Attributes = attrs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public UserType(Type type)
        {
            this.Namespace = type.Namespace;
            this.Name = type.Name;
            this.Attributes = (TypeAttributes)type.Attributes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="_namespace"></param>
        /// <param name="name"></param>
        public UserType(Type type, string _namespace, string name)
        {
            this.Namespace = _namespace;
            this.Name = name;
            this.Attributes = (TypeAttributes)type.Attributes;
        }

    }
}
