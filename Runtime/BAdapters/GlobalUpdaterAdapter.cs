using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.Updater;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(IGlobalUpdater))]
    [RequireComponent(typeof(IGlobalUpdater))]
    public class GlobalUpdaterAdapter : MonoBehaviour, IPreGameSetup
    {
        [SerializeField]
        private int preGameSetupOrder;

        int IPreGameSetup.Order => preGameSetupOrder;

        void IPreGameSetup.Execute()
        {
            IGlobalUpdater globalUpdater = GetComponent<IGlobalUpdater>();

            globalUpdater.Initialize();

            BProviderRuntime.Instance.RegisterSingleton<IGlobalUpdater, IGlobalUpdater>(globalUpdater);
        }
    }
}
