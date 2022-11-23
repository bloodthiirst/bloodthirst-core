using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.Updater;
using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(CommandManagerBehaviour))]
    [RequireComponent(typeof(CommandManagerBehaviour))]
    public class CommandManagerAdapter : MonoBehaviour, IPreGameSetup
    {
        [SerializeField]
        private int preGameSetupOrder;
        int IPreGameSetup.Order => preGameSetupOrder;

        void IPreGameSetup.Execute()
        {
            CommandManagerBehaviour commandManagerBehaviour = GetComponent<CommandManagerBehaviour>();
            commandManagerBehaviour.Initialize();

            IUpdater updater = BProviderRuntime.Instance.GetSingleton<DefaultUpdaterBehaviour>();
            
            updater.Register(commandManagerBehaviour);

            BProviderRuntime.Instance.RegisterSingleton<CommandManagerBehaviour, ICommandManagerProvider>(commandManagerBehaviour);
        }


    }
}
