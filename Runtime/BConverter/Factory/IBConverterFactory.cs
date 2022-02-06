using System;

namespace Bloodthirst.BDeepCopy
{
    internal interface IBConverterFactory
    {
        void Initialize();
        IBConverterInternal CreateConverter(Type t);
    }
}
