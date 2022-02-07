using System;

namespace Bloodthirst.BDeepCopy
{
    public interface IBConverterProvider
    {
        IBConverterFactory Factory { get; }

        IBConverter Get(Type t);
    }
}
