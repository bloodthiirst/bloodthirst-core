using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IGameStateSaver
    {
        bool CanSave(GameObject entity);
        ISaveState GenerateGameSave(IRuntimeState state);
        ISaveState GetSave(GameObject entity, SavingContext context);
    }
}