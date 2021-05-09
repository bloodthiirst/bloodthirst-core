using System;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal interface IBCopierOverrideInternal
    {
        Type AttributeType { get; }
        internal object CopyOverride(in object original, MemberInfo memberInfo, CopierSettingAttribute settingAttributen, IBCopierInternal copierForType);
    }

    internal interface IBCopierOverrideInternal<TAttribute> : IBCopierOverrideInternal where TAttribute : CopierSettingAttribute
    {
        internal object CopyOverride(in object original, MemberInfo memberInfo, TAttribute attribute, IBCopierInternal copierForType);
    }
}
