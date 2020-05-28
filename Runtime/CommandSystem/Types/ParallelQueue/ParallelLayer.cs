using System;
using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public class ParallelLayer
    {
        private List<ICommandBase> commandBasesPerLayer = default;

        private List<ICommandBase> waitableCommmands = default;

        private List<ICommandBase> interruptableCommmands = default;
        public List<ICommandBase> WaitableCommmands => waitableCommmands;

        public List<ICommandBase> InterruptableCommmands => interruptableCommmands;

        public List<ICommandBase> CommandBasesPerLayer => commandBasesPerLayer;

        public bool IsDone => IsDoneInternal();

        public ParallelLayer()
        {
            commandBasesPerLayer = new List<ICommandBase>();
            waitableCommmands = new List<ICommandBase>();
        }

        private bool IsDoneInternal()
        {
            for(int i = 0; i < waitableCommmands.Count; i++)
            {
                if (!waitableCommmands[i].IsDone)
                    return false;
            }

            return true;
        }

        public void Append(ICommandBase commandBase, bool waitForEnd , bool forceEndOnDone)
        {
            commandBasesPerLayer.Add(commandBase);

            if (waitForEnd)
            {
                waitableCommmands.Add(commandBase);
            }

            if(forceEndOnDone)
            {
                interruptableCommmands.Add(commandBase);
            }
        }

        public virtual ParallelLayer GenerateNext()
        {
            return null;
        }
    }
}
