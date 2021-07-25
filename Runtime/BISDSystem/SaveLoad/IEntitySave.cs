using Bloodthirst.Core.BISDSystem;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntitySave<TSave, TState> where TState : IEntityState where TSave : IEntityGameData<TState>
    {
        TSave GetSave(TState save, SavingContext context);
    }
}