namespace Bloodthirst.BDeepCopy
{
    public static class BConverterManger
    {
        public static BJsonProvider Json { get; private set; } = new BJsonProvider();
        public static BCopyProvider Copy { get; private set; } = new BCopyProvider();
        public static BBytesProvider Binary { get; private set; } = new BBytesProvider();
    }
}
