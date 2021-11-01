using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IBehaviourData<DATA> where DATA : EntityData
    {
        DATA Data { get; }
    }
    public interface IBehaviourData
    {
        Type Type { get; }
        EntityData Data { get; }
    }
}
