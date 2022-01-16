namespace Bloodthirst.BDeepCopy
{
    public interface IBByteConverter : IBConverter
    {
        byte[] To(object t, BConverterSettings settings);
        object From(byte[] t, BConverterSettings settings);
    }
}
