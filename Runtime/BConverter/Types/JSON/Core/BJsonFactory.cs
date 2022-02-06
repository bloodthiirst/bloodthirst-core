using System;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonFactory : BConverterFactory<IBJsonConverterInternal>
    {
        protected override IBJsonConverterInternal CreateConverterInternal(Type t)
        {
            return new BJsonComplexConverter(t);
        }
    }
}