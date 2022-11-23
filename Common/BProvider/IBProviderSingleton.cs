using System;

namespace Bloodthirst.Core.BProvider
{
    public interface IBProviderSingleton
    {
        event Action<object,object> OnChanged;
        object Value { get; set; }
    }
}