using Bloodthirst.Core.TreeList;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.ServiceProvider
{
    internal class TypeInfo
    {
        public Type MainType { get; set; }
        public List<Type> TreeParentsList { get; set; }
        public TreeList<Type,List<object>> InstanceTree { get; set; }
        public TreeLeaf<Type,List<object>> InstanceLeaf { get; set; }
        public TreeList<Type, object> SingletonTree { get; set; }
        public TreeLeaf<Type, object> SingletonLeaf { get; set; }
    }
}