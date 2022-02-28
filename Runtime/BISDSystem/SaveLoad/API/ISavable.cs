using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface ISavable
    {
        Type SavableStateType { get; }
        ISavableIdentifier GetIdentifierInfo();
        ISavableState GetSavableState();
        void ApplyState(ISavableState state);
    }
}
