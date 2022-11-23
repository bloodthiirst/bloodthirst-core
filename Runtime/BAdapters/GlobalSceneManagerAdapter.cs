using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Setup;
using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(GlobalSceneManager))]
    [RequireComponent(typeof(GlobalSceneManager))]
    public class GlobalSceneManagerAdapter : MonoBehaviour, IPreGameSetup , IPostGameEnd
    {
        [SerializeField]
        private int preGameSetupOrder;
        int IPreGameSetup.Order => preGameSetupOrder;

        void IPreGameSetup.Execute()
        {
            GlobalSceneManager bhv = GetComponent<GlobalSceneManager>();

            // dependencies
            ICommandManagerProvider commandManager = BProviderRuntime.Instance.GetSingleton<ICommandManagerProvider>();
            LoadingManager loadingManager = BProviderRuntime.Instance.GetSingleton<LoadingManager>();

            bhv.Initialize(commandManager , loadingManager);

            BProviderRuntime.Instance.RegisterSingleton(bhv);
        }

        void IPostGameEnd.Execute()
        {
           
        }
    }
}
