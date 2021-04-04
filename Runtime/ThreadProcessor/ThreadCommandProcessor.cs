﻿using Bloodthirst.Core.Singleton;
using Sirenix.OdinInspector;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Bloodthirst.Core.ThreadProcessor
{
    public class ThreadCommandProcessor : UnitySingleton<ThreadCommandProcessor>
    {
        [ShowInInspector]
        private List<ISeparateThreadCommand> threadCommand;


        private List<ISeparateThreadCommand> ThreadCommands
        {
            get
            {
                if (threadCommand == null)
                {
                    threadCommand = new List<ISeparateThreadCommand>();
                }
                return threadCommand;
            }
        }

        [ShowInInspector]
        private ConcurrentQueue<IMainThreadCommand> mainThreadCommand;


        private ConcurrentQueue<IMainThreadCommand> MainThreadCommand
        {
            get
            {
                if (mainThreadCommand == null)
                {
                    mainThreadCommand = new ConcurrentQueue<IMainThreadCommand>();
                }
                return mainThreadCommand;
            }
        }

        public void Append(ISeparateThreadCommand threadCommand)
        {
            ThreadCommands.Add(threadCommand);
            threadCommand.Start();
        }

        public void Append(IMainThreadCommand threadCommand)
        {
            MainThreadCommand.Enqueue(threadCommand);
        }

        private void LateUpdate()
        {
            // execute command in other threads

            bool done = false;

            while (done == false)
            {
                done = CheckForDoneThreaded();
            }

            // execute command in main thread

            ExecuteMainThreadCommands();

        }

        private bool CheckForDoneThreaded()
        {
            for (int i = 0; i < ThreadCommands.Count; i++)
            {
                if (ThreadCommands[i].IsDone)
                {
                    ThreadCommands[i].ExecuteCallback();
                    ThreadCommands.RemoveAt(i);
                    return false;
                }
            }

            return true;
        }

        private bool ExecuteMainThreadCommands()
        {
            while (MainThreadCommand.Count != 0)
            {
                if (MainThreadCommand.TryDequeue(out var cmd))
                    cmd?.ExecuteCallback();
            }

            return true;
        }
    }
}
