using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IGameStateLoader<TSave, TState> : IGameStateLoader
        where TState : ISavableState
        where TSave : ISavableGameSave
    {
        TState GetState(TSave save, LoadingContext context);
        void LinkReferences(LoadingInfo loadingInfo, LoadingContext context);
    }

    public interface IGameStateLoader
    {
        Type From { get; }
        Type To { get; }
        ISavableState GetState(ISavableGameSave save, LoadingContext context);
        void LinkReferences(LoadingInfo loadingInfo, LoadingContext context);
    }
}
