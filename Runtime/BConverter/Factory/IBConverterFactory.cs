using System;

namespace Bloodthirst.BDeepCopy
{
    public interface IBConverterFactory
    {
        void Initialize();
        IBConverter CreateConverter(Type t);
    }
}
