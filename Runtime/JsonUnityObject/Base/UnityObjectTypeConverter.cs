using System;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using Bloodthirst.Core.Utils;
using Bloodthirst.JsonUnityObject;

public class UnityObjectTypeConverter : TypeConverter
{
    internal CustomContext CustomContext { get; set; }
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return TypeUtils.IsSubTypeOf(sourceType, typeof(UnityEngine.Object));
    }

    // Overrides the ConvertFrom method of TypeConverter.
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        int index = int.Parse((string) value);
        UnityEngine.Object ins = CustomContext.UnityObjects[index];
        return ins;
    }

    // Overrides the ConvertTo method of TypeConverter.
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        int index = CustomContext.UnityObjects.Count;
        CustomContext.UnityObjects.Add((UnityEngine.Object)value);
        return index.ToString();
    }
}