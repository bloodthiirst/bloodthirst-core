using System;

namespace Bloodthirst.BDeepCopy
{
    public abstract class BByteBase<TFrom> : BConverter<TFrom , byte[]> , IBByteConverter
    {
        protected static readonly Type copyType = typeof(TFrom);

        object IBByteConverter.From(byte[] t, BConverterSettings settings)
        {
            return ConvertFrom(t, settings);
        }

        byte[] IBByteConverter.To(object t, BConverterSettings settings)
        {
            return ConvertTo((TFrom)t, settings);
        }
    }
}
