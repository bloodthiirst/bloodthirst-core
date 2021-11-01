using Bloodthirst.Core.BISDSystem;
using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntitySaver<TSave, TState>: IEntitySaver 
        where TState : IEntityState 
        where TSave : IEntityGameSave<TState>
    {
        TSave GetSave(TState state, SavingContext context);
    }
    
    public interface IEntitySaver
    {
        Type From { get; }
        Type To { get; }
        IEntityGameSave GetSave(IEntityState state, SavingContext context);
    }
}