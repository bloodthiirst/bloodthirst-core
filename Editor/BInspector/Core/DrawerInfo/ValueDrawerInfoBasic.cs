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
        public MemberData MemberData { get; set; }
        public MemberInfo MemberInfo => MemberData.MemberInfo;

        public object Get()
        {
            return MemberData.Getter(ContainingInstance);
        }

        public void Set(object value)
        {
            MemberData.Setter(ContainingInstance, value);
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
