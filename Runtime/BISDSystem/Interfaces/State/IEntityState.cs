namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntityState<T> : IEntityState where T : EntityData
    {
        T Data { get; set; }
    }

    public interface IEntityState : ISavableState
    {
        int Id { get; set; }

        void InitDefaultState();
    }
}
