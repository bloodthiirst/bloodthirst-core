using System;

namespace Bloodthirst.BDeepCopy
{
    internal interface IBConverterProvider
    {
        IBConverterFactory Factory { get; }

        IBConverter Get(Type t);
    }
}
