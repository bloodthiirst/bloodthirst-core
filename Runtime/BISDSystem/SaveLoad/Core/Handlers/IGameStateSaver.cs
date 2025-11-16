using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IGameStateSaver
    {
        bool CanSave(GameObject entity);
        object GenerateGameSave(object state);
        object GetSave(GameObject entity, SavingContext context);
    }
}