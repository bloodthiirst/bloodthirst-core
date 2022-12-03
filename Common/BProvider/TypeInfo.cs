using Bloodthirst.Core.TreeList;
#if ODIN_INSPECTOR
	using Sirenix.Serialization;
#endif
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.BProvider
{
    internal class TypeInfo
    {
        #if ODIN_INSPECTOR[OdinSerialize]#endif
        private TreeList<Type, IBProviderSingleton> singletonTree;

        #if ODIN_INSPECTOR[OdinSerialize]#endif
        private TreeLeaf<Type, IBProviderSingleton> singletonLeaf;

        #if ODIN_INSPECTOR[OdinSerialize]#endif
        private TreeLeaf<Type, IBProviderList> instanceLeaf;

        #if ODIN_INSPECTOR[OdinSerialize]#endif
        private TreeList<Type, IBProviderList> instanceTree;

        #if ODIN_INSPECTOR[OdinSerialize]#endif
        private List<Type> treeParentsList;

        #if ODIN_INSPECTOR[OdinSerialize]#endif
        private Type mainType;

        internal Type MainType { get => mainType; set => mainType = value; }

        /// <summary>
        /// <para>Contains a list of the inheritance hierarchy for the type</para>
        /// <para>The types it the list depend of the type (Interface or class) </para>
        /// </summary>
        internal List<Type> TreeParentsList { get => treeParentsList; set => treeParentsList = value; }

        /// <summary>
        /// Reference to the tree containing the instances
        /// </summary>
        internal TreeList<Type, IBProviderList> InstanceTree { get => instanceTree; set => instanceTree = value; }

        /// <summary>
        /// A cached reference to the leaf of the instances list of MainType
        /// </summary>
        internal TreeLeaf<Type, IBProviderList> InstanceLeaf { get => instanceLeaf; set => instanceLeaf = value; }

        /// <summary>
        /// Reference to the tree cvontaining the singletons
        /// </summary>
        internal TreeList<Type, IBProviderSingleton> SingletonTree { get => singletonTree; set => singletonTree = value; }

        /// <summary>
        /// A cached reference to the leaf of the singleton of MainType
        /// </summary>
        internal TreeLeaf<Type, IBProviderSingleton> SingletonLeaf { get => singletonLeaf; set => singletonLeaf = value; }

    }
}