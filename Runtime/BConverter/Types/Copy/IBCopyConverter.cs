namespace Bloodthirst.BDeepCopy
{
    public interface IBCopyConverter : IBConverter
    {
        object To(object t, BConverterSettings settings);
        object From(object t, BConverterSettings settings);
    }
}
