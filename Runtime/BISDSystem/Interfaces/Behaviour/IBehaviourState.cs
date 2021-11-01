using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IBehaviourState<STATE> where STATE : IEntityState
    {
        STATE State { get; }
    }
    public interface IBehaviourState
    {
        Type Type { get; }
        IEntityState State { get; }
    }
}
