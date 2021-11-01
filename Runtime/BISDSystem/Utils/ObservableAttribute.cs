using System;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ObservableAttribute : Attribute
    {
        // This is a positional argument
        public ObservableAttribute()
        {
        }
    }
}
