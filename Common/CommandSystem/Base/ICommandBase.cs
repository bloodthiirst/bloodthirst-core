using System;

namespace Bloodthirst.System.CommandSystem
{
    public struct CommandDebugInfo
    {
        public string AddedFromFilepath;
        public int AddedFromLine;
    }

    public interface ICommandBase
    {
        CommandDebugInfo DebugInfo { get; set; }

        event Action<ICommandBase> OnCommandStart;

        event Action<ICommandBase> OnCommandEnd;
        object Owner { get; set; }

        COMMAND_STATE CommandState { get; set; }

        int UpdateOrder { get; set; }

        bool RemoveWhenDone { get; set; }

        ICommandBase FallbackCommand { get; set; }

        void Start();

        void OnStart();

        void OnTick(float delta);

        void OnEnd();

        void Interrupt();

        ICommandBase GetExcutingCommand();


    }
}