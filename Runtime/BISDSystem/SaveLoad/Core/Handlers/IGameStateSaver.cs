using Bloodthirst.Core.BISDSystem;
using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IGameStateSaver<TSave, TState>: IGameStateSaver 
        where TState : ISavableState
        where TSave : ISavableGameSave
    {
        TSave GetSave(TState state, SavingContext context);
    }
    
    public interface IGameStateSaver
    {
        Type From { get; }
        Type To { get; }
        ISavableGameSave GetSave(ISavableState state, SavingContext context);
    }
}