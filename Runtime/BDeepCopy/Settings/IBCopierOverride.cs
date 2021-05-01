using System;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public interface IBCopierOverride
    {
        Type AttributeType { get; }
        internal object CopyOverride(in object original , MemberInfo memberInfo , CopierSettingAttribute settingAttribute);
    }

    internal interface IBCopierOverride<TAttribute> : IBCopierOverride where TAttribute : CopierSettingAttribute
    {
        internal object CopyOverride(in object original, MemberInfo memberInfo , TAttribute attribute);
    }
}
