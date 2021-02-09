using DummyLib;
using System;

namespace DummyLib
{
    /// <summary>
    /// A UserMethod That Takes In <see cref="DummyLib.IMethodCreator"/>, <seealso cref="string"/>
    /// </summary>
    public class UserMethod
    {
        /// <summary>
        /// 
        /// </summary>
        public IMethodCreator Method { get; set; }

        /// <summary>
        /// The methods name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="methodName"></param>
        public UserMethod(IMethodCreator method, string methodName)
        {
            this.Method = method;
            this.MethodName = methodName;
        }

        

    }

}
