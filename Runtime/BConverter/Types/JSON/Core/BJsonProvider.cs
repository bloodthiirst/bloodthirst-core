namespace Bloodthirst.BDeepCopy
{
    internal class BJsonProvider : BConverterProvider<IBJsonConverterInternal>
    {
        public BJsonProvider() : base(new BJsonFactory())
        {
            Add(new BJsonIntConverter());
            Add(new BJsonStringConverter());
        }
    }
}
