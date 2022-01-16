using System;

namespace Bloodthirst.BDeepCopy
{
    public abstract class BJsonBase<TFrom> : BConverter<TFrom, string>, IBJsonConverter
    {
        protected static readonly Type copyType = typeof(TFrom);
        string IBJsonConverter.To(object t, BConverterSettings settings)
        {
            return ConvertTo((TFrom)t, settings);
        }
        object IBJsonConverter.From(string t, BConverterSettings settings)
        {
            return ConvertFrom(t, settings);
        }
    }
}
