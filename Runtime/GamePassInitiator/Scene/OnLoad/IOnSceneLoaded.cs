using Bloodthirst.Core.SceneManager;
using Bloodthirst.System.CommandSystem;
using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.Scripts.Core.GamePassInitiator
{
    public interface IPostLoadCommand
    {
        IEnumerable<ICommandBase> GetPostLoadCommands();
    }

    public interface IOnSceneLoaded
    {
        void OnLoaded(ISceneInstanceManager sceneInstance);
    }
}
