using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IInitializeInstance
    {
        Type StateType { get; }
        void InitializeInstance(EntityIdentifier entityIdentifier , IEntityState preloadState);
    }
}
