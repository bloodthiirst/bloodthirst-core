namespace Bloodthirst.Core.AdvancedPool
{
    public interface IPoolProcessor<TObject>
    {
        void BeforeGet(TObject obj);
        void BeforeReturn(TObject obj);
    }
}