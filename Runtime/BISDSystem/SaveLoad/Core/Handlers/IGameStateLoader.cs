using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IGameStateLoader
    {
        bool CanLoad(GameObject entity, ISaveState save);
        IRuntimeState ApplyState(GameObject entity, ISaveState save, LoadingContext context);
        void LinkReferences(GameObject entity, IRuntimeState save, LoadingContext context);
    }

}
