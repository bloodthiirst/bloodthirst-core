using Bloodthirst.Core.BISDSystem;
using System;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class EntityGameSaveHandlerBase<TSave, TState> :
    IEntitySaver<TSave, TState>,
    IEntityLoader<TSave, TState>
    where TState : IEntityState
    where TSave : IEntityGameSave<TState>
    {
        private static readonly Type saveType = typeof(TSave);
        private static readonly Type stateType = typeof(TState);

        Type IEntityLoader.From => saveType;
        Type IEntityLoader.To => stateType;

        Type IEntitySaver.From => stateType;
        Type IEntitySaver.To => saveType;

        public abstract TSave GetSave(TState save, SavingContext context);
        public abstract TState GetState(TSave save, LoadingContext context);
        public abstract void LinkReferences(TSave save, TState state, LoadingContext context);

        TSave IEntitySaver<TSave, TState>.GetSave(TState state, SavingContext context)
        {
            return GetSave(state , context);
        }

        TState IEntityLoader<TSave, TState>.GetState(TSave save, LoadingContext context)
        {
            return GetState(save, context);
        }
        void IEntityLoader<TSave, TState>.LinkReferences(TSave save, TState state, LoadingContext context)
        {
            LinkReferences(save, state, context);
        }

        IEntityGameSave IEntitySaver.GetSave(IEntityState state, SavingContext context)
        {
            return GetSave((TState)state, context);
        }

        IEntityState IEntityLoader.GetState(IEntityGameSave save, LoadingContext context)
        {
            return GetState((TSave)save, context);
        }


        void IEntityLoader.LinkReferences(IEntityGameSave save, IEntityState state, LoadingContext context)
        {
            LinkReferences((TSave) save ,(TState) state, context);
        }
    }
}