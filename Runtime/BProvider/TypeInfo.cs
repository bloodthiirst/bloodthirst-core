using Bloodthirst.Core.TreeList;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.ServiceProvider
{
    internal struct TypeInfo : IEquatable<TypeInfo>
    {
        public Type MainType { get; set; }
        public List<Type> ConcreateSubTypes { get; set; }
        public List<Type> InterfaceTypes { get; set; }

        public TreeList<Type,List<object>> InstanceTree { get; set; }
        public TreeList<Type, object> SingletonTree { get; set; }

        bool IEquatable<TypeInfo>.Equals(TypeInfo other)
        {
            return MainType == other.MainType;
        }
    }
}