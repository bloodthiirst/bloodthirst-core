using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public interface ISavableSpawnInfo
    {
        GameObject GetInstanceToInject();

        void PostStatesApplied(GameObject gameObject);
    }
}
