using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    public class CommandManagerAdapter : MonoBehaviour, IPreGameSetup
    {
        int IPreGameSetup.Order => 0;
        void IPreGameSetup.Execute()
        {
            CommandManagerBehaviour behaviour = GetComponent<CommandManagerBehaviour>();

            behaviour.Initialize();
            BProviderRuntime.Instance.RegisterSingleton<CommandManagerBehaviour, CommandManagerBehaviour>(behaviour);
        }
    }
}
