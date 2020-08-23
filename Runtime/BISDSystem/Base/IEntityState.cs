namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntityState<T> : IEntityState where T : EntityData
    {
        T Data { get; set; }
    }

    public interface IEntityState
    {
        int Id { get; set; }
    }
}
