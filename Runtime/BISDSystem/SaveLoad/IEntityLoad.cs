using Bloodthirst.Core.BISDSystem;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntityLoad<TSave, TState> where TState : IEntityState where TSave : IEntityGameData<TState>
    {
        TState GetState(TSave save, LoadingContext context);
    }
}
