using System;
using System.Reflection;

namespace Bloodthirst.Editor.BInspector
{
    public interface IValueDrawerInfo
    {
        MemberInfo MemberInfo { get; }
        object GetContainingInstance();
        object Get();

        void Set(object value);

        Type DrawerType();
    }
}
