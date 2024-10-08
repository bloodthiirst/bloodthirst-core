using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface ISavable
    {
        Type SavableStateType { get; }
        ISavableState GetSavableState();
        void ApplyState(ISavableState state);
    }
}
