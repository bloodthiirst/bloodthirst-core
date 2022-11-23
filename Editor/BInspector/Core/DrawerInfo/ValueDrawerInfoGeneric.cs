using System;
using System.Reflection;

namespace Bloodthirst.Editor.BInspector
{
    public struct ValueProviderGeneric : IValueProvider
    {
        private ValuePath valuePath;
        private object instance;
        private readonly MemberInfo memberInfo;
        internal readonly object state;
        private Func<object> get;
        private Action<object> set;
        private readonly Type drawerType;
        public ValuePath ValuePath => valuePath;
        public MemberInfo MemberInfo => memberInfo;

        public ValueProviderGeneric(object state, ValuePath valuePath, Func<object> get, Action<object> set, Type drawerType, object instance, MemberInfo memberInfo)
        {
            this.set = set;
            this.drawerType = drawerType;
            this.state = state;
            this.get = get;
            this.instance = instance;
            this.memberInfo = memberInfo;
            this.valuePath = valuePath;
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
