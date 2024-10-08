using System;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class GameSaveHandlerBase<TSave, TState> :
    IGameStateSaver<TSave, TState>,
    IGameStateLoader<TSave, TState>
    where TState : ISavableState
    where TSave : ISavableGameSave
    {
        private static readonly Type saveType = typeof(TSave);
        private static readonly Type stateType = typeof(TState);

        Type IGameStateLoader.From => saveType;
        Type IGameStateLoader.To => stateType;

        Type IGameStateSaver.From => stateType;
        Type IGameStateSaver.To => saveType;

        public abstract TSave GetSave(TState state, SavingContext context);
        public abstract TState GetState(TSave save, LoadingContext context);
        public abstract void LinkReferences(LoadingInfo loadingInfo, LoadingContext context);

        TSave IGameStateSaver<TSave, TState>.GetSave(TState state, SavingContext context)
        {
            return GetSave(state, context);
        }

        TState IGameStateLoader<TSave, TState>.GetState(TSave save, LoadingContext context)
        {
            return GetState(save, context);
        }
        void IGameStateLoader<TSave, TState>.LinkReferences(LoadingInfo loadingInfo, LoadingContext context)
        {
            LinkReferences(loadingInfo, context);
        }

        ISavableGameSave IGameStateSaver.GetSave(ISavableState state, SavingContext context)
        {
            return GetSave((TState)state, context);
        }

        ISavableState IGameStateLoader.GetState(ISavableGameSave save, LoadingContext context)
        {
            return GetState((TSave)save, context);
        }

        void IGameStateLoader.LinkReferences(LoadingInfo loadingInfo, LoadingContext context)
        {
            LinkReferences(loadingInfo, context);
        }
    }
}