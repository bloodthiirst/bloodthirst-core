namespace Assets.Scripts.BISDSystem
{
    public interface IInstanceProvider
    {
        void Register<T>(T instance);

        T Get<T>();
    }
}
