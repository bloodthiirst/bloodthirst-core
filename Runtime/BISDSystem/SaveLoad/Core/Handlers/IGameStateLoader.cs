using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IGameStateLoader<TSave, TState> : IGameStateLoader
        where TState : ISavableState
        where TSave : ISavableGameSave
    {
        TState GetState(TSave save, LoadingContext context);
        void LinkReferences(TSave save, TState state, LoadingContext context);
    }

    public interface IGameStateLoader
    {
        Type From { get; }
        Type To { get; }
        ISavableState GetState(ISavableGameSave save, LoadingContext context);
        void LinkReferences(ISavableGameSave save, ISavableState state, LoadingContext context);
    }
}
