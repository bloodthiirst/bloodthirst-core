using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public abstract class SceneManagerProviderBehaviourBase : MonoBehaviour
    {
        public abstract ISceneInstanceManager GetSceneManager();
    }
}
