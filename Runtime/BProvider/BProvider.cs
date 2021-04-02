using Bloodthirst.Core.TreeList;
using Bloodthirst.System.Quadrant;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.ServiceProvider
{
    public class BProvider
    {
        private Dictionary<Type, TypeInfo> TypeToInfoHash { get; set; }

        private TreeList<Type,object> TypeTree { get; set; }

        public BProvider()
        {
            TypeToInfoHash = new Dictionary<Type, TypeInfo>();
            TypeTree = new TreeList<Type, object>();
        }

        public void RegisterInstance<T>(T instance)
        {
            Type t = typeof(T);

            TypeInfo info = GetInfo(t);

            TypeTree.AddElement( info.ConcreateSubTypes, instance);
        }

        public IEnumerable<T> GetInstances<T>()
        {
            Type t = typeof(T);
            return TypeTree.GetElements(t).Cast<T>();
        }

        private TypeInfo GetInfo(Type t)
        {
            if (!TypeToInfoHash.TryGetValue(t , out TypeInfo info))
            {
                info = FlattenType(t);
                TypeToInfoHash.Add(t, info);
                return info;
            }

            return info;
        }

        public void RegisterType(Type t)
        {
            if (TypeToInfoHash.ContainsKey(t))
                return;

            TypeToInfoHash.Add(t, FlattenType(t));
        }

        public void RegisterType<T>()
        {
            Type t = typeof(T);

            if (TypeToInfoHash.ContainsKey(t))
                return;

            TypeToInfoHash.Add(t, FlattenType(t));
        }

        /// <summary>
        /// Take a type and returns the flattened list of subtypes
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private TypeInfo FlattenType(Type type)
        {
            List<Type> concreatSubTypes = new List<Type>();

            Type curr = type;

            while (curr != typeof(object))
            {
                concreatSubTypes.Add(curr);
                curr = curr.BaseType;
            }

            TypeInfo typeInfo = new TypeInfo()
            {
                MainType = type,
                ConcreateSubTypes = concreatSubTypes,
                InterfaceTypes = type.GetInterfaces().ToList()
            };

            return typeInfo;
        }
    }


    internal struct TypeInfo : IEquatable<TypeInfo>
    {
        public Type MainType { get; set; }
        public List<Type> ConcreateSubTypes { get; set; }
        public List<Type> InterfaceTypes { get; set; }

        bool IEquatable<TypeInfo>.Equals(TypeInfo other)
        {
            return MainType == other.MainType;
        }
    }


}