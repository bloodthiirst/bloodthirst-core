namespace Assets.Scripts.BISDSystem
{
    public interface IInstanceProvider
    {
        T Get<T>();
    }
}
