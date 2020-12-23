using System;
using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public interface ICommandBase
    {

        event Action<ICommandBase> OnCommandStart;

        event Action<ICommandBase> OnCommandEnd;

        COMMAND_STATE CommandState { get; set; }
        
        bool IsStarted { get; }
        
        bool IsDone { get; }
        
        ICommandBase FallbackCommand { get; set; }

        void Start();

        void OnStart();

        void OnTick(float delta);

        void OnEnd();

        ICommandBase GetExcutingCommand();

        void Interrupt();
    }
}