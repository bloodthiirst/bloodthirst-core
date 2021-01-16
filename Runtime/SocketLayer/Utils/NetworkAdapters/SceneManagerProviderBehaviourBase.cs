using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public abstract class SceneManagerProviderBehaviourBase : MonoBehaviour
    {
        public abstract ISceneInstanceManager GetSceneManager();
    }
}
