using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public abstract class GameSaveHandlerBase<TSave, TState> :
    IGameStateSaver,
    IGameStateLoader
    where TSave : ISaveState
    where TState : IRuntimeState
    {

        public abstract bool CanLoad(GameObject entity, ISaveState save);
        public abstract bool CanSave(GameObject entity);
        public abstract TSave GenerateGameSave(TState state);
        public abstract TSave GetSave(GameObject entity, SavingContext context);
        public abstract TState ApplyState(GameObject entity, TSave save, LoadingContext context);
        public abstract void LinkReferences(GameObject entity, TState state, LoadingContext context);

        IRuntimeState IGameStateLoader.ApplyState(GameObject entity, ISaveState save, LoadingContext context)
        {
            TSave castedSave = (TSave)save;
            TState castedState = ApplyState(entity, castedSave, context);
            return castedState;
        }

        void IGameStateLoader.LinkReferences(GameObject entity, IRuntimeState state, LoadingContext context)
        {
            LinkReferences(entity, (TState)state, context);
        }

        ISaveState IGameStateSaver.GetSave(GameObject entity, SavingContext context)
        {
            TSave castedSave = GetSave(entity, context);
            return castedSave;
        }

        bool IGameStateSaver.CanSave(GameObject entity)
        {
            return CanSave(entity);
        }

        bool IGameStateLoader.CanLoad(GameObject entity, ISaveState save)
        {
            return CanLoad(entity, (ISaveState)save);
        }

        ISaveState IGameStateSaver.GenerateGameSave(IRuntimeState state)
        {
            return GenerateGameSave((TState)state);
        }
    }
}