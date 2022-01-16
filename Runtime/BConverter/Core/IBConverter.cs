using System;

namespace Bloodthirst.BDeepCopy
{
    public interface IBConverter
    {
        Type FromType { get; }
        Type ToType { get; }

        object ConvertFrom(object t , BConverterSettings bCopierSettings = null);
        object ConvertTo(object t , BConverterSettings bCopierSettings = null);
    }

    internal interface IBConverterInternal : IBConverter
    {
        object ConvertFrom_Internal(object t, BConverterContext copierContext , BConverterSettings bCopierSettings);
        object ConvertTo_Internal(object t, BConverterSettings bCopierSettings = null);
        object GetDefaultValue();
    }

    public interface IBConverter<TFrom , TTo>
    {
        TFrom ConvertFrom(TTo t, BConverterSettings bCopierSettings = null);
        TTo ConvertTo(TFrom t, BConverterSettings bCopierSettings = null);
    }

}
