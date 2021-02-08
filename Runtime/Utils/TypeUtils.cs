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
                if (allTypes == null)
                {
                    allTypes = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .SelectMany(asm => asm.GetTypes())
                                .ToList();
                }

                return allTypes;
            }
        }

        /// <summary>
        /// Is <paramref name="child"/>  a subtype (or same type) as <paramref name="parent"/> ?
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsSubTypeOf(Type child, Type parent)
        {
            if (child == parent)
                return true;

            return parent.IsAssignableFrom(child);
        }

        /// <summary>
        /// Is <typeparamref name="T"/> a subtype (or same type) as <paramref name="parent"/> ?
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsSubTypeOf<T>(Type parent)
        {
            Type child = typeof(T);
            return IsSubTypeOf(child, parent);
        }

        /// <summary>
        /// Is <typeparamref name="T"/> a subtype (or same type) as <typeparamref name="K"/> ?
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsSubTypeOf<T, K>()
        {
            Type child = typeof(T);
            Type parent = typeof(K);
            return IsSubTypeOf(child, parent);
        }
    }
}
