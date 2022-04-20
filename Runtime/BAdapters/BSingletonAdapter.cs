using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Singleton;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    public class BSingletonAdapter : MonoBehaviour, ISetupSingletonPass
    {
        void ISetupSingletonPass.Execute()
        {
            // todo : make this attached to spawned singletons
            IBSingleton singleton = GetComponent<IBSingleton>();

            BProviderRuntime.Instance.RegisterSingleton(singleton.Concrete , singleton);

            if (singleton.Concrete != singleton.Interface)
            {
                BProviderRuntime.Instance.RegisterSingleton(singleton.Interface, singleton);
            }

            singleton.OnSetupSingleton();
        }
    }
}
