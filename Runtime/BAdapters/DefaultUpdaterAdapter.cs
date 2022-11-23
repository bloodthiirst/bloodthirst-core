using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.Updater;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(DefaultUpdaterBehaviour))]
    [RequireComponent(typeof(DefaultUpdaterBehaviour))]
    public class DefaultUpdaterAdapter : MonoBehaviour, IPreGameSetup
    {
        [SerializeField]
        private int preGameSetupOrder;

        int IPreGameSetup.Order => preGameSetupOrder;

        void IPreGameSetup.Execute()
        {
            DefaultUpdaterBehaviour defaultUpdater = GetComponent<DefaultUpdaterBehaviour>();

            IGlobalUpdater global = BProviderRuntime.Instance.GetSingleton<IGlobalUpdater>();
            global.Register(defaultUpdater);

            BProviderRuntime.Instance.RegisterSingleton<DefaultUpdaterBehaviour, DefaultUpdaterBehaviour>(defaultUpdater);
        }
    }
}
