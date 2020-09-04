namespace Bloodthirst.Core.BISDSystem
{
    public interface IBehaviour<INSTANCE>
    {
        INSTANCE Instance { get; }
    }
    public interface IRemovableBehaviour
    {
        IRemovable Removable { get; }
    }

}
