using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public abstract class PrefabInstanceProviderBehaviourBase : MonoBehaviour
    {
        public abstract T GetPrefabInstance<T>() where T : Component;
        public abstract bool RemovePrefabInstance<T>(T t) where T : Component;

    }
}
