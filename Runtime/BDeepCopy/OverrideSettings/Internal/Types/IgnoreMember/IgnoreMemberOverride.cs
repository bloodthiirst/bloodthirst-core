using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal class IgnoreMemberOverride : BCopierOverrideBaseInternal<IgnoreMemberAttribute>
    {
        protected override object CopyOverride(in object original, MemberInfo memberInfo, IgnoreMemberAttribute attribute, IBCopierInternal copierForType)
        {
            return copierForType.GetDefaultValue();
        }
    }
}
