using System;

namespace Bloodthirst.BDeepCopy
{
    public interface IBConverter
    {
        Type FromType { get; }
        Type ToType { get; }

        object ConvertFrom(object t);
        object ConvertTo(object t);
    }

    public interface IBConverter<TFrom , TTo>
    {
        TFrom ConvertFrom(TTo t);
        TTo ConvertTo(TFrom t);
    }

}
