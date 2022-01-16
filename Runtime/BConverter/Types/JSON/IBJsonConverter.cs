namespace Bloodthirst.BDeepCopy
{
    public interface IBJsonConverter : IBConverter
    {
        string To(object t, BConverterSettings settings);
        object From(string t, BConverterSettings settings);
    }
}
