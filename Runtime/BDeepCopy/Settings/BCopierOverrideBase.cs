using System;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public abstract class BCopierOverrideBase<TAttribute> : IBCopierOverride<TAttribute> where TAttribute : CopierSettingAttribute
    {
        private static readonly Type attributeType = typeof(TAttribute);
        Type IBCopierOverride.AttributeType => attributeType;

        object IBCopierOverride.CopyOverride(in object original, MemberInfo memberInfo , CopierSettingAttribute settingAttribute)
        {
            return CopyOverride(in original , memberInfo , (TAttribute) settingAttribute);
        }

        protected abstract object CopyOverride(in object original, MemberInfo memberInfo , TAttribute attribute);

        object IBCopierOverride<TAttribute>.CopyOverride(in object original, MemberInfo memberInfo, TAttribute attribute)
        {
            return CopyOverride(in original, memberInfo, attribute);
        }
    }
}
