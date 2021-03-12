namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntityInstanceRegister
    {
        void Register<T>(T instance);

        void Unregister<T>(T instance);
    }
}
