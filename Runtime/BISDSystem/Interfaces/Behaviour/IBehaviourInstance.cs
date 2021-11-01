using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IBehaviourInstance<INSTANCE> where INSTANCE : IEntityInstance
    {
        INSTANCE Instance { get; }
    }

    public interface IBehaviourInstance
    {
        Type Type { get; }
        IEntityInstance Instance { get; }
    }
}
