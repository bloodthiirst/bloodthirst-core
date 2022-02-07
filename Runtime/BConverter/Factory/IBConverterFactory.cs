using System;

namespace Bloodthirst.BDeepCopy
{
    public interface IBConverterFactory
    {
        void Initialize();
        IBConverterInternal CreateConverter(Type t);
    }
}
