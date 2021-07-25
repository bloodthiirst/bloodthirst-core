using Bloodthirst.Core.BISDSystem;
using System;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class EntityLoadSaveHandler<TSave, TState> :
    IEntitySave<TSave, TState>,
    IEntityLoad<TSave, TState>
    where TState : IEntityState
    where TSave : IEntityGameData<TState>
    {
        private static readonly Type saveType = typeof(TSave);
        private static readonly Type stateType = typeof(TSave);
        public Type SaveType => saveType;
        public Type StateType => stateType;

        public abstract TSave GetSave(TState save, SavingContext context);
        public abstract TState GetState(TSave save, LoadingContext context);
    }

    public class SavingContext
    {

    }

    public class LoadingContext
    {

    }
}