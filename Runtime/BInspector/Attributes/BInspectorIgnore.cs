using System;

namespace Bloodthirst.Runtime.BInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Event, Inherited = false, AllowMultiple = false)]
    public sealed class BInspectorIgnore : Attribute
    {
    }
}
