using System;

namespace Bloodthirst.BDeepCopy
{
    [AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public abstract class CopierSettingAttribute : Attribute
    {
    }
}
