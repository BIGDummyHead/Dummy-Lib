using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyLib
{
    /// <summary>
    /// Create Method(s)
    /// </summary>
    public interface IMethodCreator
    {
        /// <summary>
        /// Methods
        /// </summary>
        Dictionary<string, Type[]> Methods { get; }
    }
}
