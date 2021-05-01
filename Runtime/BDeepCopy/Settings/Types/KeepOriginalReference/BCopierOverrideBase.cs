using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public class KeepOriginalReferenceOverride : BCopierOverrideBase<KeepOriginalReference>
    {
        protected override object CopyOverride(in object original, MemberInfo memberInfo, KeepOriginalReference attribute)
        {
            return original;
        }
    }
}
