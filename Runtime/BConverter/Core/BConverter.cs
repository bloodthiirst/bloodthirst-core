using System;

namespace Bloodthirst.BDeepCopy
{
    public abstract class BConverter<TFrom, TTo> : IBConverter<TFrom, TTo>, IBConverter
    {
        protected static readonly Type fromType = typeof(TFrom);
        protected static readonly Type toType = typeof(TTo);

        Type IBConverter.FromType => fromType;

        Type IBConverter.ToType => toType;

        public abstract TFrom ConvertFrom(TTo t, BConverterSettings settings = null);

        public abstract TTo ConvertTo(TFrom t, BConverterSettings settings = null);

        object IBConverter.ConvertFrom(object t, BConverterSettings settings)
        {
            return ConvertFrom((TTo)t, settings);
        }

        object IBConverter.ConvertTo(object t, BConverterSettings settings)
        {
            return ConvertTo((TFrom)t, settings);
        }
    }
}
