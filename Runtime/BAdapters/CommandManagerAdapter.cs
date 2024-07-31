using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.Updater;
using Bloodthirst.System.CommandSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(CommandManagerBehaviour))]
    [RequireComponent(typeof(CommandManagerBehaviour))]
    public class CommandManagerAdapter : MonoBehaviour, IPreGameSetup
    {
        [SerializeField]
        private int preGameSetupOrder;

        [SerializeField]
        private GameObject updaterObject;

        [SerializeField]
        private bool isDefaultProvider;

        int IPreGameSetup.Order => preGameSetupOrder;

        void IPreGameSetup.Execute()
        {
            CommandManagerBehaviour commandManagerBehaviour = GetComponent<CommandManagerBehaviour>();
            commandManagerBehaviour.Initialize();

            IUpdater updater = updaterObject.GetComponent<IUpdater>();
            Assert.IsNotNull(updater);

            updater.Register(commandManagerBehaviour);

            if (isDefaultProvider)
            {
                BProviderRuntime.Instance.RegisterSingleton<CommandManagerBehaviour, ICommandManagerProvider>(commandManagerBehaviour);
            }
        }
    }
}
