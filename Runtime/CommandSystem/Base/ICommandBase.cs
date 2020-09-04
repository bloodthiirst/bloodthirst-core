using System;
using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public interface ICommandBase
    {

        COMMAND_STATE CommandState { get; set; }
        bool IsStarted { get; set; }
        bool IsDone { get; set; }
        List<ICommandBase> FallbackCommands { get; set; }

        event Action OnCommandStart;

        event Action OnCommandEnd;

        void OnCommandStartNotify();

        void OnStart();
        void OnTick(float delta);
        void OnEnd();
        ICommandBase GetExcutingCommand();

        void Interrupt();

    }
}