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

        public BSingleton<TSingletonType> GetSingleton<TSingletonType>() where TSingletonType : class
        {
            Type t = typeof(TSingletonType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.SingletonLeaf == null)
            {
                info.SingletonLeaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            return (BSingleton<TSingletonType>)info.SingletonLeaf.Value;
        }

        public IEnumerable<TSingletonType> GetSingletons<TSingletonType>() where TSingletonType : class
        {
            Type t = typeof(TSingletonType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.SingletonLeaf == null)
            {
                info.SingletonLeaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            foreach (List<object> e in info.SingletonLeaf.TraverseAllSubElements())
            {
                for (int i = 0; i < e.Count; i++)
                {
                    yield return (TSingletonType)e[i];
                }
            }
        }

        public bool RegisterOrReplaceSingleton<TInstanceType>(TInstanceType instance) where TInstanceType : class
        {
            Type t = typeof(TInstanceType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.SingletonLeaf == null)
            {
                info.SingletonLeaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            if (info.SingletonLeaf.Value == null)
            {
                info.SingletonLeaf.Value = new BSingleton<TInstanceType>(instance);
                return true;
            }

            BSingleton<TInstanceType> s = (BSingleton<TInstanceType>)info.SingletonLeaf.Value;

            s.Value = instance;

            return true;
        }

        public bool RegisterOrReplaceSingleton<TInstanceType , ISingletonType>(TInstanceType instance) where ISingletonType : class where TInstanceType : ISingletonType
        {
            Type t = typeof(ISingletonType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.SingletonLeaf == null)
            {
                info.SingletonLeaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            if (info.SingletonLeaf.Value == null)
            {
                info.SingletonLeaf.Value = new BSingleton<ISingletonType>(instance);
                return true;
            }

            BSingleton<ISingletonType> s = (BSingleton<ISingletonType>)info.SingletonLeaf.Value;

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

            if (info.SingletonLeaf == null)
            {
                info.SingletonLeaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            if (info.SingletonLeaf.Value == null)
            {
                info.SingletonLeaf.Value = new BSingleton<ISingletonType>(instance);
                return true;
            }

            BSingleton<ISingletonType> s = (BSingleton<ISingletonType>)info.SingletonLeaf.Value;

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

            if (info.SingletonLeaf == null)
            {
                info.SingletonLeaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            if (info.SingletonLeaf.Value == null)
            {
                info.SingletonLeaf.Value = new BSingleton<TInstanceType>(instance);
                return true;
            }

            BSingleton<TInstanceType> s = (BSingleton<TInstanceType>)info.SingletonLeaf.Value;

            bool isValid = s.Value == instance;

            // check if it's the same instance of not
            return isValid;
        }

        public void RemoveSingleton<TInstanceType>(TInstanceType instance) where TInstanceType : class
        {
            Type t = typeof(TInstanceType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.SingletonLeaf == null)
            {
                info.SingletonLeaf = info.SingletonTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            if (info.SingletonLeaf.Value == null)
            {
                return;
            }

            info.SingletonLeaf.Value = null;
        }


        #endregion

        #region multiple instances

        public IEnumerable<TInjectionType> GetInstances<TInjectionType>() where TInjectionType : class
        {
            Type t = typeof(TInjectionType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.InstanceLeaf == null)
            {
                info.InstanceLeaf = info.InstanceTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            foreach (List<object> e in info.InstanceLeaf.TraverseAllSubElements())
            {
                for (int i = 0; i < e.Count; i++)
                {
                    yield return (TInjectionType)e[i];
                }
            }
        }


        public void RemoveInstance<TInstanceType>(TInstanceType instance) where TInstanceType : class
        {
            Type t = typeof(TInstanceType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.InstanceLeaf == null)
            {
                info.InstanceLeaf = info.InstanceTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            if (info.InstanceLeaf.Value == null)
            {
                return;
            }

            info.InstanceLeaf.Value.Remove(instance);
        }

        public void RemoveInstance<TInstanceType, TInjectionType>(TInstanceType instance) where TInstanceType : class, TInjectionType
        {
            Type t = typeof(TInjectionType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.InstanceLeaf == null)
            {
                info.InstanceLeaf = info.InstanceTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            if (info.InstanceLeaf.Value == null)
            {
                return;
            }

            info.InstanceLeaf.Value.Remove(instance);
        }

        public void RegisterInstance<TInstanceType>(TInstanceType instance) where TInstanceType : class
        {
            Type t = typeof(TInstanceType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.InstanceLeaf == null)
            {
                info.InstanceLeaf = info.InstanceTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            if (info.InstanceLeaf.Value == null)
            {
                info.InstanceLeaf.Value = new List<object>();
            }

            info.InstanceLeaf.Value.Add(instance);
        }
        public void RegisterInstance<TInstanceType, TInjectionType>(TInstanceType instance) where TInstanceType : class, TInjectionType
        {
            Type t = typeof(TInjectionType);

            TypeInfo info = GetOrCreateInfo(t);

            if (info.InstanceLeaf == null)
            {
                info.InstanceLeaf = info.InstanceTree.GetOrCreateLeaf(info.TreeParentsList);
            }

            info.InstanceLeaf = info.InstanceTree.GetOrCreateLeaf(info.TreeParentsList);

            if (info.InstanceLeaf.Value == null)
            {
                info.InstanceLeaf.Value = new List<object>();
            }

            info.InstanceLeaf.Value.Add(instance);
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