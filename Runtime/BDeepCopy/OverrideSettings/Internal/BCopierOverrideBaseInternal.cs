using System;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BCopierOverrideBaseInternal<TAttribute> : IBCopierOverrideInternal<TAttribute> where TAttribute : CopierSettingAttribute
    {
        private static readonly Type attributeType = typeof(TAttribute);

        Type IBCopierOverrideInternal.AttributeType => attributeType;

        object IBCopierOverrideInternal.CopyOverride(in object original, MemberInfo memberInfo, CopierSettingAttribute settingAttribute, IBCopierInternal copierForType)
        {
            return CopyOverride(in original, memberInfo, (TAttribute)settingAttribute , copierForType);
        }

        object IBCopierOverrideInternal<TAttribute>.CopyOverride(in object original, MemberInfo memberInfo, TAttribute attribute, IBCopierInternal copierForType)
        {
            return CopyOverride(in original, memberInfo, attribute, copierForType);
        }

        protected abstract object CopyOverride(in object original, MemberInfo memberInfo, TAttribute attribute, IBCopierInternal copierForType);

    }
}
