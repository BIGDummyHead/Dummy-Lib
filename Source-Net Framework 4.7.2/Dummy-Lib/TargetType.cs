using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyLib
{
    /// <summary>
    /// Helps the <see cref="AssemblyWriter"/> Find Types Easily
    /// </summary>
    public class TargetType
    {
        /// <summary>
        /// The name of the Target
        /// </summary>
        public string TargetName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        public TargetType(Type targetType) => (TargetName) = (targetType.FullName);

        /// <summary>
        /// Is A Reflection Name
        /// <para>Reflection : Namespace.TypeName</para>
        /// <para>Nested : Namespace.TypeName+NestedTypeName</para>
        /// </summary>
        public bool IsReflectionName { get => AssemblyWriter.IsReflectionName(TargetName); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        public TargetType(TypeDef targetType) => (TargetName, TypeDefinition) = (AssemblyWriter.GetFullName(targetType), targetType);

        /// <summary>
        /// <see cref="TypeDef"/> -- Not Null If Using <see cref="TargetType(TypeDef)"/>
        /// </summary>
        public TypeDef TypeDefinition { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetName"></param>
        public TargetType(string targetName) => (TargetName) = (targetName);

        /// <summary>
        /// Returns a Nice Overlay of your target
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string _ret = string.Empty;

            _ret = $"Target: \"{TargetName}\" \nIs Nested: {!IsReflectionName} \nIs Reflected: {IsReflectionName}";

            return _ret;
        }
    }

}
