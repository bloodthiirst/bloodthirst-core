using System;

namespace Bloodthirst.Editor.BInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class BInspectorIgnore : Attribute
    {
    }
}
