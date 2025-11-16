using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IGameStateLoader
    {
        bool CanLoad(GameObject entity, object save);
        object ApplyState(GameObject entity, object save, LoadingContext context);
        void LinkReferences(GameObject entity, object save, LoadingContext context);
    }

}
