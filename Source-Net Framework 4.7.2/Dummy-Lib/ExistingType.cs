using System;
using dnlib.DotNet;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DummyLib
{
    /// <summary>
    /// An Existing Type
    /// </summary>
    public sealed class ExistingType : UserType
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingType"></param>
        public ExistingType(Type existingType) : base(existingType)
        {
            ModuleContext con = ModuleDef.CreateModuleContext();
            ModuleDefMD module = ModuleDefMD.Load(existingType.Module, con);

            TypeDef existing = module.Find(existingType.FullName, true);

            Methods = Copy(existing.Methods);

            Events = Copy(existing.Events);

            Properties = Copy(existing.Properties);
            //this for some reasons make them not null??
            foreach (PropertyDef item in Properties)
            {
                item.GetMethod.ReturnType = item.GetMethod.ReturnType;
            }

            Fields = Copy(existing.Fields);

            TypeDef baseT = existing.ScopeType.ResolveTypeDef().BaseType?.ResolveTypeDef();

            Inherit = baseT;

            if (Inherit?.GetType()?.Name == "TypeDefMD")
            {
                Inherit = null;
            }


            Interfaces = Copy(existing.Interfaces);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingType"></param>
        /// <param name="_namespace"></param>
        /// <param name="name"></param>
        public ExistingType(Type existingType, string _namespace, string name) : base(existingType, _namespace, name)
        {
            ModuleContext con = ModuleDef.CreateModuleContext();
            ModuleDefMD module = ModuleDefMD.Load(existingType.Module, con);

            TypeDef existing = module.Find(existingType.FullName, true);

            Methods = Copy(existing.Methods);

            Events = Copy(existing.Events);

            Properties = Copy(existing.Properties);
            //this for some reasons make them not null??
            foreach (PropertyDef item in Properties)
            {
                item.GetMethod.ReturnType = item.GetMethod.ReturnType;
            }

            Fields = Copy(existing.Fields);

            Inherit = existing.BaseType;

            if (Inherit?.Name == nameof(Object))
                Inherit = null;

            Interfaces = Copy(existing.Interfaces);
        }

       
        internal ExistingType(AssemblyWriter writer, ITypeDefOrRef reffed, ITypeResolver resolver) : base(reffed.Namespace, reffed.Name, reffed.ResolveTypeDef().Attributes)
        {
            TypeDef existing = reffed.ResolveTypeDef();
            Methods = Copy(existing.Methods);
            Events = Copy(existing.Events);

            Properties = Copy(existing.Properties);
            //this for some reasons make them not null??
            foreach (PropertyDef item in Properties)
            {
                item.GetMethod.ReturnType = item.GetMethod.ReturnType;
            }

            Fields = Copy(existing.Fields);

            ITypeDefOrRef baseT = existing.ScopeType.ResolveTypeDef().BaseType;

            Inherit = baseT;


            if (string.IsNullOrEmpty(Inherit?.Name) || Inherit?.Name == nameof(Object))
            {
                Inherit = null;
            }

            Interfaces = Copy(existing.Interfaces);


        }

        /// <summary>
        /// Fields
        /// </summary>
        public IEnumerable<FieldDef> Fields { get; }

        /// <summary>
        /// Methods
        /// </summary>
        public IEnumerable<MethodDef> Methods { get; }

        /// <summary>
        /// Properties
        /// </summary>
        public IEnumerable<PropertyDef> Properties { get; }

        /// <summary>
        /// Events
        /// </summary>
        public IEnumerable<EventDef> Events { get; }

        /// <summary>
        /// Interfaces
        /// </summary>
        public IEnumerable<InterfaceImpl> Interfaces { get; }

        /// <summary>
        /// The <see cref="TypeDef"/> The Existing Type Inherits
        /// </summary>
        public ITypeDefOrRef Inherit { get; }

        private IEnumerable<T> Copy<T>(IEnumerable<T> toCopy)
        {
            List<T> copied = new List<T>();

            copied.AddRange(toCopy);

            return copied;
        }
    }
}
