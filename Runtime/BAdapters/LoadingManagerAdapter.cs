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
        int IPreGameSetup.Order => 1;
        void IPreGameSetup.Execute()
        {
            LoadingManager bhv = GetComponent<LoadingManager>();

            bhv.Initialize(BProviderRuntime.Instance.GetSingleton<CommandManagerBehaviour>());

            BProviderRuntime.Instance.RegisterSingleton(bhv);
        }

        void IPostGameEnd.Execute()
        {
            LoadingManager bhv = GetComponent<LoadingManager>();

            bhv.Interrupt();
        }
    }
}
