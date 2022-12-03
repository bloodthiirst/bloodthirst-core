using Bloodthirst.BType;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BInspector;
#if ODIN_INSPECTOR
	using Sirenix.Utilities;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Editor.BInspector
{
    public struct ValueDrawerInfoBasic : IValueProvider
    {
        public object ContainingInstance { get; set; }
        public BMemberData MemberData { get; set; }
        public MemberInfo MemberInfo => MemberData.MemberInfo;

        public ValuePath ValuePath { get; set; }

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
            return TypeUtils.GetReturnType(MemberData.MemberInfo);
        }

        public object GetContainingInstance()
        {
            return ContainingInstance;
        }
    }
}
