using Bloodthirst.Core.TreeList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Core.ServiceProvider
{
    public class BProvider
    {
        /// <summary>
        /// Dictionary cache containing info about the types
        /// </summary>
        private Dictionary<Type, TypeInfo> TypeToInfoHash { get; set; }

        /// <summary>
        /// Tree structure to store the singletons
        /// </summary>
        private TreeList<Type, object> Singletons { get; set; }

        /// <summary>
        /// Tree structure to store the instances of concreate types
        /// </summary>
        private TreeList<Type, object> Classes { get; set; }

        /// <summary>
        /// Tree structre to store interfaces
        /// </summary>
        private TreeList<Type, object> Interfaces { get; set; }

        public BProvider()
        {
            TypeToInfoHash = new Dictionary<Type, TypeInfo>();
            Classes = new TreeList<Type, object>();
            Singletons = new TreeList<Type, object>();
        }

        #region singleton
        public T GetSingleton<T>() where T : class
        {
            Type t = typeof(T);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, object> leaf = Singletons.GetOrCreateLeaf(info.ConcreateSubTypes);

            if (leaf.Elements.Count == 0)
                return null;

            return (T)leaf.Elements[0];
        }

        /// <summary>
        /// Register an instance 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool RegisterSingleton<T>(T instance) where T : class
        {
            Type t = typeof(T);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, object> leaf = Singletons.GetOrCreateLeaf(info.ConcreateSubTypes);

            if (leaf.Elements == null)
            {
                leaf.Elements = new List<object>(1) { instance };
                return true;
            }
            // check if an instance already exists
            // if not , then add and return true
            if (leaf.Elements.Count == 0)
            {
                leaf.Elements.Add(instance);
                return true;
            }

            // else check if it's the same instance of not
            return leaf.Elements[0] == instance;
        }
        #endregion

        #region concrete instances

        public IEnumerable<T> GetInstances<T>() where T : class
        {
            Type t = typeof(T);
            TypeInfo info = GetOrCreateInfo(t);
            return info.ValidTypeTree.GetElementsRecursivly(t).Cast<T>();
        }


        public void RegisterInstance<T>(T instance) where T : class
        {
            Type t = typeof(T);

            TypeInfo info = GetOrCreateInfo(t);

            info.ValidTypeTree.AddElement(info.ConcreateSubTypes, instance);
        }

        #endregion

        #region common

        /// <summary>
        /// Get of create info about a certain type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private TypeInfo GetOrCreateInfo(Type t)
        {
            if (!TypeToInfoHash.TryGetValue(t, out TypeInfo info))
            {
                info = FlattenType(t);
                TypeToInfoHash.Add(t, info);
                return info;
            }

            return info;
        }

        public void RegisterType(Type t)
        {
            GetOrCreateInfo(t);
        }

        public void RegisterType<T>()
        {
            Type t = typeof(T);

            RegisterType(t);
        }

        /// <summary>
        /// Take a type and returns the flattened list of subtypes
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private TypeInfo FlattenType(Type type)
        {            
            Type curr = type;

            TreeList<Type, object> validTree = curr.IsInterface ? Interfaces : Classes;

            List<Type> concreatSubTypes = new List<Type>();

            while (curr != typeof(object))
            {
                concreatSubTypes.Add(curr);
                curr = curr.BaseType;
            }

            TypeInfo typeInfo = new TypeInfo()
            {
                MainType = type,
                ConcreateSubTypes = concreatSubTypes,
                InterfaceTypes = type.GetInterfaces().ToList(),
                ValidTypeTree = validTree
            };

            return typeInfo;
        }
    }

    #endregion
}