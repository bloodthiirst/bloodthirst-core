using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class GameSaveHandlerBase<TSave, TState> :
    IGameStateSaver,
    IGameStateLoader
    where TSave : ISavableGameSave
    where TState : ISavableState
    {

        public abstract bool CanLoad(GameObject entity, ISavableGameSave save);
        public abstract bool CanSave(GameObject entity);
        public abstract TSave GenerateGameSave(TState state);
        public abstract TSave GetSave(GameObject entity, SavingContext context);
        public abstract TState ApplyState(GameObject entity, TSave save, LoadingContext context);
        public abstract void LinkReferences(GameObject entity, TState state, LoadingContext context);

        object IGameStateLoader.ApplyState(GameObject entity, object save, LoadingContext context)
        {
            TSave castedSave = (TSave)save;
            TState castedState = ApplyState(entity, castedSave, context);
            return castedState;
        }

        void IGameStateLoader.LinkReferences(GameObject entity, object state, LoadingContext context)
        {
            LinkReferences(entity, (TState)state, context);
        }

        object IGameStateSaver.GetSave(GameObject entity, SavingContext context)
        {
            TSave castedSave = GetSave(entity, context);
            return castedSave;
        }

        bool IGameStateSaver.CanSave(GameObject entity)
        {
            return CanSave(entity);
        }

        bool IGameStateLoader.CanLoad(GameObject entity, object save)
        {
            return CanLoad(entity, (ISavableGameSave)save);
        }

        object IGameStateSaver.GenerateGameSave(object state)
        {
            return GenerateGameSave((TState)state);
        }
    }
}