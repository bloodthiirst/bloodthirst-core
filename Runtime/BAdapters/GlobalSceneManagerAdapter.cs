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
        int IPreGameSetup.Order => 1;

        void IPreGameSetup.Execute()
        {
            GlobalSceneManager bhv = GetComponent<GlobalSceneManager>();

            // dependencies
            CommandManagerBehaviour commandManager = BProviderRuntime.Instance.GetSingleton<CommandManagerBehaviour>();
            LoadingManager loadingManager = BProviderRuntime.Instance.GetSingleton<LoadingManager>();

            bhv.Initialize(commandManager , loadingManager);

            BProviderRuntime.Instance.RegisterSingleton(bhv);
        }

        void IPostGameEnd.Execute()
        {
           
        }
    }
}
