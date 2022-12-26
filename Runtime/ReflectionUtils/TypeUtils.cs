#if ODIN_INSPECTOR
	using Sirenix.Utilities;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.Core.Utils
{
    /// <summary>
    /// an extension class containing helper methods for types / reflection
    /// </summary>
    public static class TypeUtils
    {
        private static List<Type> allTypes;

        public readonly static Type[] PrimitiveTypes = new Type[]
        {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(IntPtr),
            typeof(UIntPtr),
            typeof(char),
            typeof(double),
            typeof(float)
        };

        public static IEnumerable<Type> GetBaseClasses(this Type type)
        {
            var t = type.BaseType;
            while(t != null)
            {
                yield return t;
                t = t.BaseType;
            }
        }

        // in later .NETs, you can cache reflection extensions using a static generic class and
        // a ConcurrentDictionary. E.g.
        //public static class Attributes<T> where T : Attribute
        //{
        //    private static readonly ConcurrentDictionary<MemberInfo, IReadOnlyCollection<T>> _cache =
        //        new ConcurrentDictionary<MemberInfo, IReadOnlyCollection<T>>();
        //
        //    public static IReadOnlyCollection<T> Get(MemberInfo member)
        //    {
        //        return _cache.GetOrAdd(member, GetImpl, Enumerable.Empty<T>().ToArray());
        //    }
        //    //GetImpl as per code below except that recursive steps re-enter via the cache
        //}

        public static List<T> GetAttributesWithInheritence<T>(this MemberInfo member) where T : Attribute
        {
            List<T> attributes = new List<T>();

            List<Type> allTypes = new List<Type>();

            Type parentType = member.DeclaringType;
            List<Type> allClasses = parentType.GetBaseClasses().ToList();
            List<Type> allInterfaces = parentType.GetInterfaces().ToList();

            allTypes.Add(parentType);
            allTypes.AddRange(allClasses);
            allTypes.AddRange(allInterfaces);

            foreach(Type i in allTypes)
            {
                foreach(MemberInfo mem in i.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (mem.MemberType != member.MemberType || mem.Name != member.Name)
                        continue;

                    IEnumerable<T> attrsFound = mem.GetCustomAttributes<T>(true);
                    attributes.AddRange(attrsFound);
                }
            }

            return attributes;
        }

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

        public static List<MethodInfo> GetAllMethods(Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            List<MethodInfo> methods = new List<MethodInfo>();

            while (true)
            {
                methods.AddRange(typeInfo.DeclaredMethods);

                Type type2 = typeInfo.BaseType;

                if (type2 == null)
                {
                    break;
                }

                typeInfo = type2.GetTypeInfo();
            }

            return methods;
        }

        /// <summary>
        /// <para>Does type <paramref name="t"/> have a class that inherits it ?</para>
        /// <para>NOTE : This doesn't guarantee a correct return value if the type <paramref name="t"/> passed is a generic type</para>
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool HasSubClass(Type t)
        {
            if (t.IsSealed)
                return false;

            if (t.IsValueType)
                return false;

            if (t.IsGenericType)
                return true;

            // if it's already inherited by another class
            if (AllTypes.FirstOrDefault(c => IsSubTypeOf(c, t)) != null)
                return true;

            return false;
        }

        /// <summary>
        /// <para>Is the class of type <paramref name="t"/> a pure value type ?</para>
        /// <para>NOTE : a "pure value type" is a struct (OR <see cref="string"/> )  that contains only copiable value members (that can be passed by value directly)</para>
        /// <list type="bullet">
        /// <listheader>
        /// <term> Basic primitives </term>
        /// <description> Base pure types </description>
        /// </listheader>
        /// <see cref="bool"/>
        /// <see cref="byte"/>
        /// <see cref="sbyte"/> 
        /// <see cref="short"/>
        /// <see cref="ushort"/>
        /// <see cref="int"/> 
        /// <see cref="uint"/>
        /// <see cref="long"/>
        /// <see cref="ulong"/>
        /// <see cref="IntPtr"/>
        /// <see cref="UIntPtr"/>
        /// <see cref="char"/>
        /// <see cref="float"/>
        /// <see cref="double"/>
        /// <see cref="string"/>
        /// </list>
        /// </summary>
        /// <param name="t"></param>
        public static bool IsPureValueType(Type t)
        {
            if (t.IsArray)
                return false;

            if (t.IsInterface)
                return false;

            if (t.IsPrimitive)
                return true;

            if (t == typeof(string))
                return true;

            if (t.IsClass)
                return false;

            if (t.IsEnum)
                return true;

            foreach (FieldInfo prop in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!IsPureValueType(prop.FieldType))
                    return false;
            }

            return true;
        }

        private static bool IsPureValueTypeInternal(Type t, HashSet<Type> cache)
        {
            if (t.IsArray)
                return false;

            if (t.IsPrimitive)
                return true;

            if (t == typeof(string))
                return true;

            if (t.IsClass)
                return false;

            if (t.IsEnum)
                return true;

            foreach (FieldInfo prop in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!cache.Add(prop.FieldType))
                    continue;

                if (!IsPureValueTypeInternal(prop.FieldType, cache))
                    return false;
            }

            return true;
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


        private static string GetNiceNameRecursive(Type t)
        {
            if(t == typeof(int))
            {
                return "int";
            }
            if (t == typeof(bool))
            {
                return "bool";
            }
            if (t == typeof(string))
            {
                return "string";
            }
            if (t == typeof(float))
            {
                return "float";
            }
            if (t == typeof(double))
            {
                return "double";
            }
            if (t == typeof(short))
            {
                return "short";
            }

            // List<T> for example
            if (t.IsGenericTypeDefinition)
            {
                return t.Name;
            }

            if(t.IsGenericType)
            {
                string str = t.Name + "<";

                for(int i = 0; i < t.GenericTypeArguments.Length; i++)
                {
                    str += GetNiceNameRecursive(t.GenericTypeArguments[i]);
                    str += ",";
                }

                str.TrimEnd(',');
                str += ">";

                return str;
            }

            return t.Name;
        }

        public static string GetNiceName(Type t)
        {
            return GetNiceNameRecursive(t);

        }


        //
        // Summary:
        //     FieldInfo will return the fieldType, propertyInfo the PropertyType, MethodInfo
        //     the return type and EventInfo will return the EventHandlerType.
        //
        // Parameters:
        //   memberInfo:
        //     The MemberInfo.
        public static Type GetReturnType(this MemberInfo memberInfo)
        {
            FieldInfo fieldInfo = memberInfo as FieldInfo;
            if ((object)fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            if ((object)propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            MethodInfo methodInfo = memberInfo as MethodInfo;
            if ((object)methodInfo != null)
            {
                return methodInfo.ReturnType;
            }

            return (memberInfo as EventInfo)?.EventHandlerType;
        }
    }
}
