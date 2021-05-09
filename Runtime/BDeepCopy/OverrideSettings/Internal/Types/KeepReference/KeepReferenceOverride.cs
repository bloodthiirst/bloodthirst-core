using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal class KeepReferenceOverride : BCopierOverrideBaseInternal<KeepReferenceAttribute>
    {
        protected override object CopyOverride(in object original, MemberInfo memberInfo, KeepReferenceAttribute attribute, IBCopierInternal copierForType)
        {
            return original;
        }
    }
}
