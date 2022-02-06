using System;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BConverter<TFrom, TTo> : IBConverter<TFrom, TTo> , IBConverterInternal
    {
        protected static readonly Type fromType = typeof(TFrom);
        protected static readonly Type toType = typeof(TTo);

        Type IBConverter.FromType => fromType;

        Type IBConverter.ToType => toType;

        public IBConverterProvider Provider { get; set; }

        public TFrom ConvertFrom(TTo t)
        {
            return ConvertFrom(t, new BConverterContext(), new BConverterSettings());
        }
        public TTo ConvertTo(TFrom t)
        {
            return ConvertTo(t, new BConverterContext(), new BConverterSettings());
        }

        public abstract void Initialize();
        protected abstract TFrom ConvertFrom(TTo t , BConverterContext context, BConverterSettings settings);

        protected abstract TTo ConvertTo(TFrom t , BConverterContext context, BConverterSettings settings);

        object IBConverter.ConvertFrom(object t)
        {
            return ConvertFrom((TTo)t);
        }
        object IBConverter.ConvertTo(object t)
        {
            return ConvertTo((TFrom)t);
        }
        object IBConverterInternal.ConvertFrom_Internal(object t, BConverterContext context, BConverterSettings settings)
        {
            return ConvertFrom((TTo)t, context, settings);
        }

        object IBConverterInternal.ConvertTo_Internal(object t, BConverterContext context, BConverterSettings settings)
        {
            return ConvertTo((TFrom) t, context, settings);
        }
    }
}
