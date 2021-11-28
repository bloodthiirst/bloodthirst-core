using Bloodthirst.Editor.BInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Editor.BInspector
{
    public struct DrawerInfoGeneric : IDrawerInfo
    {
        internal readonly object state;
        private Func<object> get;
        private Action<object> set;
        private readonly Type drawerType;

        public DrawerInfoGeneric(object state, Func<object> get, Action<object> set , Type drawerType)
        {
            this.set = set;
            this.drawerType = drawerType;
            this.state = state;
            this.get = get;
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
    }
    public struct DrawerInfo : IDrawerInfo
    {
        public object ContainerInstance { get; set; }
        public MemberData MemberData { get; set; }
        public MemberInfo memberInfo => MemberData.MemberInfo;

        public object Get()
        {
            return MemberData.Getter(ContainerInstance);
        }

        public void Set(object value)
        {
            MemberData.Setter(ContainerInstance, value);
        }

        public Type DrawerType()
        {
            return MemberData.MemberInfo.GetReturnType();
        }
    }

    public interface IDrawerInfo
    {
        object Get();

        void Set(object value);

        Type DrawerType();
    }
}
