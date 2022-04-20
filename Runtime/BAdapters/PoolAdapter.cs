using Bloodthirst.Core.AdvancedPool;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    public class PoolAdapter : MonoBehaviour, IBeforeAllScenesInitializationPass
    {
        void IBeforeAllScenesInitializationPass.Execute()
        {
            IPoolBehaviour pool = GetComponent<IPoolBehaviour>();

            pool.Populate();
        }
    }
}
