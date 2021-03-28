using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntityInstance
    {
        EntityIdentifier EntityIdentifier { get; }
        Type StateType { get; }
        Type InstanceType { get; }
        IEntityState State { get; set; }
    }
}
