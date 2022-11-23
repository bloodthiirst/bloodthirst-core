using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Setup;
using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(LoadingManager))]
    [RequireComponent(typeof(LoadingManager))]
    public class LoadingManagerAdapter : MonoBehaviour, IPreGameSetup , IPostGameEnd
    {
        [SerializeField]
        private int preGameSetupOrder;
        int IPreGameSetup.Order => preGameSetupOrder;
        void IPreGameSetup.Execute()
        {
            LoadingManager bhv = GetComponent<LoadingManager>();

            ICommandManagerProvider commandManager = BProviderRuntime.Instance.GetSingleton<ICommandManagerProvider>();
            bhv.Initialize(commandManager);

            BProviderRuntime.Instance.RegisterSingleton(bhv);
        }

        void IPostGameEnd.Execute()
        {
            LoadingManager bhv = GetComponent<LoadingManager>();

            bhv.Interrupt();
        }
    }
}
