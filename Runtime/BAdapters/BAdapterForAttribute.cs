using System;

namespace Bloodthirst.Runtime.BAdapter
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class BAdapterForAttribute : Attribute
    {
        public Type AdapterForType { get; private set; }

        public BAdapterForAttribute(Type adapterForType)
        {
            AdapterForType = adapterForType;
        }
    }
}
