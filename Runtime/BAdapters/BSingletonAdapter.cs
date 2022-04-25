using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Singleton;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(IBSingleton))]
    [RequireComponent(typeof(IBSingleton))]
    public class BSingletonAdapter : MonoBehaviour, ISetupSingletonPass
    {
        void ISetupSingletonPass.Execute()
        {
            IBSingleton[] allSingletons = GetComponents<IBSingleton>();

            foreach (IBSingleton singleton in allSingletons)
            {
                BProviderRuntime.Instance.RegisterSingleton(singleton.Concrete, singleton);

                if (singleton.Concrete != singleton.Interface)
                {
                    BProviderRuntime.Instance.RegisterSingleton(singleton.Interface, singleton);
                }

                singleton.OnSetupSingleton();
            }
        }
    }
}
