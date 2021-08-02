using Bloodthirst.Core.BISDSystem;
using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntityLoader<TSave, TState> : IEntityLoader where TState : IEntityState where TSave : IEntityGameData<TState>
    {
        TState GetState(TSave save, LoadingContext context);
        void LinkReferences(TSave save, TState state, LoadingContext context);
    }

    public interface IEntityLoader
    {
        Type From { get; }
        Type To { get; }
        IEntityState GetState(IEntityGameData save, LoadingContext context);
        void LinkReferences(IEntityGameData save , IEntityState state, LoadingContext context);
    }
}
