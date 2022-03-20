using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BType
{
    public struct BMemberData
    {
        public Type Type { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public Func<object, object> MemberGetter { get; set; }
        public Action<object, object> MemberSetter { get; set; }
        public Dictionary<Type, Attribute> DirectAttributes { get; set; }
        public Dictionary<Type, List<Attribute>> InheritedAttributes { get; internal set; }
    }
}
