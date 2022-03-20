using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public interface ISavableInstanceProvider
    {
        GameObject GetInstanceToInject();

        void PostStatesApplied(GameObject gameObject);
    }
}
