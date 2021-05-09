using System;

namespace Bloodthirst.BDeepCopy
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public abstract class CopierSettingAttribute : Attribute
    {
    }
}
