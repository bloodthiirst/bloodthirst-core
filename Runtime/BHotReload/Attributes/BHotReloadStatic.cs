using System;

namespace Bloodthirst.Runtime.BHotReload
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class BHotReloadStatic : Attribute
    {
    }
}
