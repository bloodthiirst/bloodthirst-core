using System;

namespace Bloodthirst.BJson
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class BJsonIgnoreAttribute : Attribute
    {
    }
}
