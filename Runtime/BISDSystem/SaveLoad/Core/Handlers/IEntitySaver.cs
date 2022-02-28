using Bloodthirst.Core.BISDSystem;
using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntitySaver<TSave, TState>: IEntitySaver 
        where TState : ISavableState
        where TSave : ISavableGameSave
    {
        TSave GetSave(TState state, SavingContext context);
    }
    
    public interface IEntitySaver
    {
        Type From { get; }
        Type To { get; }
        ISavableGameSave GetSave(ISavableState state, SavingContext context);
    }
}