namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntityInstanceProvider
    {
        T Get<T>() where T : class;
    }
}
