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

        #region class properties
        /// <summary>
        /// Tree structure to store the singletons
        /// </summary>
        private TreeList<Type, object> ClassSingletons { get; set; }

        /// <summary>
        /// Tree structure to store the instances of concreate types
        /// </summary>
        private TreeList<Type, List<object>> ClassInstances { get; set; }
        #endregion

        #region interface properties
        /// <summary>
        /// Tree structure to store the singletons
        /// </summary>
        private TreeList<Type, object> InterfaceSingletons { get; set; }

        /// <summary>
        /// Tree structre to store interfaces
        /// </summary>
        private TreeList<Type, List<object>> InterfaceInstances { get; set; }
        #endregion

        public BProvider()
        {
            TypeToInfoHash = new Dictionary<Type, TypeInfo>();
            
            ClassInstances = new TreeList<Type, List<object>>();
            ClassSingletons = new TreeList<Type, object>();

            InterfaceInstances = new TreeList<Type, List<object>>();
            InterfaceSingletons = new TreeList<Type, object>();
        }

        #region singleton
        public BSingleton<TSingleton> GetSingleton<TSingleton>() where TSingleton : class
        {
            Type t = typeof(TSingleton);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, object> leaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);

            return (BSingleton<TSingleton>)leaf.Value;
        }


        public bool RegisterOrReplaceSingleton<TInstanceType>(TInstanceType instance) where TInstanceType : class
        {
            Type t = typeof(TInstanceType);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, object> leaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);

            if (leaf.Value == null)
            {
                leaf.Value = new BSingleton<TInstanceType>(instance);
                return true;
            }

            BSingleton<TInstanceType> s = (BSingleton<TInstanceType>)leaf.Value;

            s.Value = instance;

            return true;
        }

        public bool RegisterOrReplaceSingleton<TInstanceType , ISingletonType>(TInstanceType instance) where ISingletonType : class where TInstanceType : ISingletonType
        {
            Type t = typeof(ISingletonType);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, object> leaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);

            if (leaf.Value == null)
            {
                leaf.Value = new BSingleton<ISingletonType>(instance);
                return true;
            }

            BSingleton<ISingletonType> s = (BSingleton<ISingletonType>)leaf.Value;

            s.Value = instance;

            return true;
        }

        /// <summary>
        /// Register an instance 
        /// </summary>
        /// <typeparam name="TInstanceType"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool RegisterSingleton<TInstanceType , ISingletonType>(TInstanceType instance) where ISingletonType : class where TInstanceType : ISingletonType
        {
            Type t = typeof(ISingletonType);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, object> leaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);

            if (leaf.Value == null)
            {
                leaf.Value = new BSingleton<ISingletonType>(instance);
                return true;
            }

            BSingleton<ISingletonType> s = (BSingleton<ISingletonType>) leaf.Value;

            // check if it's the same instance of not
            return s.Value == (ISingletonType) instance;
        }

        /// <summary>
        /// Register an instance 
        /// </summary>
        /// <typeparam name="TInstanceType"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool RegisterSingleton<TInstanceType>(TInstanceType instance) where TInstanceType : class
        {
            Type t = typeof(TInstanceType);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, object> leaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);

            if (leaf.Value == null)
            {
                leaf.Value = new BSingleton<TInstanceType>(instance);
                return true;
            }

            BSingleton<TInstanceType> s = (BSingleton<TInstanceType>)leaf.Value;

            // check if it's the same instance of not
            return s.Value == instance;
        }

        #endregion

        #region concrete instances

        public IEnumerable<TInjectionType> GetInstances<TInjectionType>() where TInjectionType : class
        {
            Type t = typeof(TInjectionType);

            TypeInfo info = GetOrCreateInfo(t);

            foreach (List<object> e in info.InstanceTree.GetElementsRecursivly(t))
            {
                for (int i = 0; i < e.Count; i++)
                {
                    yield return (TInjectionType)e[i];
                }
            }
        }

        public void RegisterInstance<TInstanceType>(TInstanceType instance) where TInstanceType : class
        {
            Type t = typeof(TInstanceType);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, List<object>> leaf = info.InstanceTree.GetOrCreateLeaf(info.TreeParentsList);

            if (leaf.Value == null)
            {
                leaf.Value = new List<object>();
            }

            leaf.Value.Add(instance);
        }
        public void RegisterInstance<TInstanceType, TInjectionType>(TInstanceType instance) where TInstanceType : class, TInjectionType
        {
            Type t = typeof(TInjectionType);

            TypeInfo info = GetOrCreateInfo(t);

            TreeLeaf<Type, List<object>> leaf = info.InstanceTree.GetOrCreateLeaf(info.TreeParentsList);

            if (leaf.Value == null)
            {
                leaf.Value = new List<object>();
            }

            leaf.Value.Add(instance);
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

            bool isInsterface = curr.IsInterface;


            TreeList<Type, List<object>> instanceTree = null;
            TreeList<Type, object> singletonTree = null;

            switch (isInsterface)
            {
                case true:
                    instanceTree = InterfaceInstances;
                    singletonTree = InterfaceSingletons;
                    break;
                default:
                    instanceTree = ClassInstances;
                    singletonTree = ClassSingletons;
                    break;
            }

            List<Type> concreatSubTypes = new List<Type>();

            concreatSubTypes.Add(curr);

            if (!isInsterface)
            {
                curr = curr.BaseType;

                while (curr != typeof(object))
                {
                    concreatSubTypes.Add(curr);
                    curr = curr.BaseType;
                }
            }

            else
            {
                concreatSubTypes.AddRange(type.GetInterfaces().ToList());
            }

            TypeInfo typeInfo = new TypeInfo()
            {
                MainType = type,
                TreeParentsList = concreatSubTypes,
                InstanceTree = instanceTree,
                SingletonTree = singletonTree
            };

            return typeInfo;
        }
    }

    #endregion
}