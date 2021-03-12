namespace Bloodthirst.Core.BISDSystem
{
    public interface IInstanceInjector<INSTANCE>
    {
        INSTANCE GetInstance();
    }
}
