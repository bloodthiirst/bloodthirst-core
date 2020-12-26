using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Core.Utils
{
    /// <summary>
    /// an extension class containing helper methods for types / reflection
    /// </summary>
    public static class TypeUtils
    {
        private static List<Type> allTypes;

        /// <summary>
        /// Get a readonly list of all the types in every assembly
        /// </summary>
        public static IReadOnlyList<Type> AllTypes
        {
            get
            {
                if(allTypes == null)
                {
                    allTypes = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .SelectMany(asm => asm.GetTypes())
                                .ToList();
                }

                return allTypes;
            }
        }
    }
}
