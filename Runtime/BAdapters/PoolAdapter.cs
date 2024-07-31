using Bloodthirst.Core.AdvancedPool;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(IPoolBehaviour))]
    [RequireComponent(typeof(IPoolBehaviour))]
    public class PoolAdapter : MonoBehaviour, IOnSceneLoaded
    {
        void IOnSceneLoaded.OnLoaded(ISceneInstanceManager sceneInstance)
        {
            IPoolBehaviour pool = GetComponent<IPoolBehaviour>();

            pool.Populate();
        }
    }
}
