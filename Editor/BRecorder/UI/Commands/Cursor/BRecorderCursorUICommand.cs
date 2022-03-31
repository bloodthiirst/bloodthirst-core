using Bloodthirst.Editor.BInspector;
using Bloodthirst.Runtime.BRecorder;
using UnityEditor;
using UnityEngine.UIElements;

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
