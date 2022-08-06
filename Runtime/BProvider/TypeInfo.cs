using Bloodthirst.Core.TreeList;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.BProvider
{
    internal class TypeInfo
    {
        internal Type MainType { get; set; }

        /// <summary>
        /// <para>Contains a list of the inheritance hierarchy for the type</para>
        /// <para>The types it the list depend of the type (Interface or class) </para>
        /// </summary>
        internal List<Type> TreeParentsList { get; set; }

        /// <summary>
        /// Reference to the tree containing the instances
        /// </summary>
        internal TreeList<Type, IBProviderList> InstanceTree { get; set; }

        /// <summary>
        /// A cached reference to the leaf of the instances list of MainType
        /// </summary>
        internal TreeLeaf<Type, IBProviderList> InstanceLeaf { get; set; }

        /// <summary>
        /// Reference to the tree cvontaining the singletons
        /// </summary>
        internal TreeList<Type, object> SingletonTree { get; set; }

        /// <summary>
        /// A cached reference to the leaf of the singleton of MainType
        /// </summary>
        internal TreeLeaf<Type, object> SingletonLeaf { get; set; }

    }
}