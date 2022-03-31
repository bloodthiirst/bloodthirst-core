using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Runtime.BRecorder
{
    public class BRecorderSession
    {
        public event Action<IBRecorderCommand> OnCommandAdded;
        public event Action<IBRecorderCommand> OnCommandRemoved;
        public event Action<BRecorderSession> OnSessionChanged;

        [OdinSerialize]
        private List<IBRecorderCommand> RecorderCommands { get; set; }

        public IReadOnlyList<IBRecorderCommand> Commands => RecorderCommands;

        public BRecorderSession()
        {
            RecorderCommands = new List<IBRecorderCommand>();
        }

        public void Add(IBRecorderCommand recorderCommand)
        {
            RecorderCommands.Add(recorderCommand);

            OnCommandAdded?.Invoke(recorderCommand);
            OnSessionChanged?.Invoke(this);
        }

        public void Remove(IBRecorderCommand recorderCommand)
        {
            RecorderCommands.Remove(recorderCommand);
            OnCommandRemoved?.Invoke(recorderCommand);
            OnSessionChanged?.Invoke(this);
        }

        public void Clear()
        {
            for (int i = RecorderCommands.Count - 1; i >= 0; i--)
            {
                IBRecorderCommand cmd = RecorderCommands[i];
                RecorderCommands.RemoveAt(i);

                OnCommandRemoved?.Invoke(cmd);
                OnSessionChanged?.Invoke(this);
            }
        }
    }
}
