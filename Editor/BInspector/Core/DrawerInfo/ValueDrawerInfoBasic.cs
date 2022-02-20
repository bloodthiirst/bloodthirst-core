using Bloodthirst.BType;
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
    public struct ValueDrawerInfoBasic : IValueDrawerInfo
    {
        public object ContainingInstance { get; set; }
        public BMemberData MemberData { get; set; }
        public MemberInfo MemberInfo => MemberData.MemberInfo;

        public object Get()
        {
            return MemberData.MemberGetter(ContainingInstance);
        }

        public void Set(object value)
        {
            MemberData.MemberSetter(ContainingInstance, value);
        }

        public Type DrawerType()
        {
            return MemberData.MemberInfo.GetReturnType();
        }

        public object GetContainingInstance()
        {
            return ContainingInstance;
        }
    }
}
