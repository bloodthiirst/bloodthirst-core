using System;

namespace Bloodthirst.BDeepCopy
{
    public abstract class BCopyBase<TFrom> : BConverter<TFrom , TFrom> , IBCopyConverter
    {
        protected static readonly Type copyType = typeof(TFrom);

        object IBCopyConverter.From(object t, BConverterSettings settings)
        {
            return ConvertFrom((TFrom) t, settings);
        }

        object IBCopyConverter.To(object t, BConverterSettings settings)
        {
            return ConvertTo((TFrom) t, settings);
        }
    }
}
