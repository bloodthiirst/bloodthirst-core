using System;
using System.Reflection;

namespace Bloodthirst.Editor.BInspector
{
    public interface IValueProvider
    {
        ValuePath ValuePath { get; }
        MemberInfo MemberInfo { get; }
        object GetContainingInstance();
        object Get();
        void Set(object value);
        Type DrawerType();
    }
}
