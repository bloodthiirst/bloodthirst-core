using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public struct BMemberData
    {
        public Type Type { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public Func<object, object> MemberGetter { get; set; }
        public Action<object, object> MemberSetter { get; set; }
        public Dictionary<Type, Attribute> Attributes { get; set; }
    }
}
