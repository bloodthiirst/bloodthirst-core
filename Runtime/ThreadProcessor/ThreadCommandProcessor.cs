using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
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
        private Queue<IMainThreadCommand> mainThreadCommand;


        private Queue<IMainThreadCommand> MainThreadCommand
        {
            get
            {
                if (mainThreadCommand == null)
                {
                    mainThreadCommand = new Queue<IMainThreadCommand>();
                }
                return mainThreadCommand;
            }
        }

        public static void Append(ISeparateThreadCommand threadCommand)
        {
            Instance.ThreadCommands.Add(threadCommand);
            threadCommand.Start();
        }

        public static void Append(IMainThreadCommand threadCommand)
        {
            Instance.MainThreadCommand.Enqueue(threadCommand);
        }

        private void FixedUpdate()
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
                MainThreadCommand.Dequeue()?.ExecuteCallback();
            }

            return true;
        }
    }
}
