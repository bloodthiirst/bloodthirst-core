using System;
using System.Reflection;

namespace Bloodthirst.Editor.BInspector
{
    public struct ValueDrawerInfoGeneric : IValueDrawerInfo
    {
        private object instance;
        private readonly MemberInfo memberInfo;
        internal readonly object state;
        private Func<object> get;
        private Action<object> set;
        private readonly Type drawerType;

        public MemberInfo MemberInfo => memberInfo;

        public ValueDrawerInfoGeneric(object state , Func<object> get, Action<object> set, Type drawerType, object instance , MemberInfo memberInfo = null)
        {
            this.set = set;
            this.drawerType = drawerType;
            this.state = state;
            this.get = get;
            this.instance = instance;
            this.memberInfo = memberInfo;
        }

        public Type DrawerType()
        {
            return drawerType;
        }

        public object Get()
        {
            return get();
        }

        public void Set(object value)
        {
            set(value);
        }

        public object GetContainingInstance()
        {
            return instance;
        }
    }
}
