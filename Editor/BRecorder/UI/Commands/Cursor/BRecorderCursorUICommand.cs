using Bloodthirst.Runtime.BRecorder;

namespace Bloodthirst.Editor.BRecorder
{
    public class BRecorderCursorUICommand : BRecorderCursorUIBase
    {
        public IBRecorderCommand Command { get; private set; }

        public BRecorderCursorUICommand(IBRecorderCommand command) : base(CURSOR_TYPE.COMMAND)
        {
            Command = command;
        }

    }
}
