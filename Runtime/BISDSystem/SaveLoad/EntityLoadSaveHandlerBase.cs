using Bloodthirst.Core.BISDSystem;
using System;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class EntityLoadSaveHandlerBase<TSave, TState> :
    IEntitySaver<TSave, TState>,
    IEntityLoader<TSave, TState>
    where TState : IEntityState
    where TSave : IEntityGameData<TState>
    {
        private static readonly Type saveType = typeof(TSave);
        private static readonly Type stateType = typeof(TState);
        public Type SaveType => saveType;
        public Type StateType => stateType;

        Type IEntityLoader.From => saveType;
        Type IEntityLoader.To => stateType;

        Type IEntitySaver.From => stateType;
        Type IEntitySaver.To => saveType;

        public abstract TSave GetSave(TState save, SavingContext context);
        public abstract TState GetState(TSave save, LoadingContext context);

        TSave IEntitySaver<TSave, TState>.GetSave(TState state, SavingContext context)
        {
            return GetSave(state , context);
        }

        TState IEntityLoader<TSave, TState>.GetState(TSave save, LoadingContext context)
        {
            return GetState(save, context);
        }

        IEntityGameData IEntitySaver.GetSave(IEntityState state, SavingContext context)
        {
            return GetSave((TState)state, context);
        }

        IEntityState IEntityLoader.GetState(IEntityGameData save, LoadingContext context)
        {
            return GetState((TSave)save, context);
        }
    }

    public class SavingContext
    {

    }

    public class LoadingContext
    {
        public T GetInstance<T>(int id) where T : IEntityInstance
        {
            return default;
        }
    }
}