namespace Bloodthirst.BDeepCopy
{
    public interface IBConverterInternal : IBConverter
    {
        IBConverterProvider Provider { get; set; }
        void Initialize();
        object ConvertFrom_Internal(object t, BConverterContext context, BConverterSettings settings);
        object ConvertTo_Internal(object t, BConverterContext context, BConverterSettings settings);
    }

}
