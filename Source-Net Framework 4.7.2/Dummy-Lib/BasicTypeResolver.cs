using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace DummyLib
{
    /// <summary>
    /// Type Import Data
    /// </summary>
    public struct ResolveData
    {
        /// <summary>
        /// Namespace of the Type - Can return null or <see cref="string.Empty"/>
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Name of the Type
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Type to import
        /// </summary>
        public ITypeDefOrRef ImportType { get; }

        /// <summary>
        /// The Full Name Of The Type
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Import Data On <see cref="Type"/> <paramref name="t"/>
        /// </summary>
        /// <param name="t"></param>
        public ResolveData(ITypeDefOrRef t)
        {
            if (t != null)
            {
                FullName = t.FullName;
                TypeName = t.Name;
                this.Namespace = t.Namespace;
                ImportType = t;
            }
            else
            {
                FullName = null;
                TypeName = null;
                this.Namespace = null;
                ImportType = null;
            }



        }
    }

    /// <summary>
    /// Resolves imported Assemblies
    /// </summary>
    public class BasicTypeResolver : ITypeResolver
    {
        private List<string> _resolved = new List<string>();

        /// <summary>
        /// The Resolved Type Names
        /// </summary>
        public string[] ResolvedTypes { get => _resolved.ToArray(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="rd"></param>
        /// <param name="resolver"></param>
        /// <param name="_namespace">The new namespace of the resolved Type</param>
        /// <returns></returns>
        public virtual TypeDef ResolveType(AssemblyWriter writer, ResolveData rd, string _namespace, ITypeResolver resolver)
        {
            if (rd.ImportType is null)
                return default;

            if (_resolved.Contains(rd.FullName))
                return default;

            _resolved.Add(rd.FullName);

            return writer.AddExistingType(rd.ImportType, _namespace, resolver);
        }
    }

    /// <summary>
    /// Used for Resolving Assemblies -- 
    /// Used By <see cref="AssemblyWriter"/>
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        /// Resolve A Type 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="rd"></param>
        /// <param name="_namespace"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        TypeDef ResolveType(AssemblyWriter writer, ResolveData rd, string _namespace, ITypeResolver resolver);
    }
}
