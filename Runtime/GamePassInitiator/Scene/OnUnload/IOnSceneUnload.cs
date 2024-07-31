using Bloodthirst.Core.SceneManager;
using Bloodthirst.System.CommandSystem;
using System.Collections.Generic;

namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IPreUnloadCommand
    {
        IEnumerable<ICommandBase> GetPreUnloadCommands();
    }

    public interface IOnSceneUnload
    {
        void OnUnload(ISceneInstanceManager sceneInstance);
    }
}
