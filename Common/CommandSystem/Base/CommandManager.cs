#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandManager
    {

        
#if ODIN_INSPECTOR
[ShowInInspector]
#endif

        private List<List<ICommandBase>> commandBatches;

        public CommandManager()
        {
            commandBatches = new List<List<ICommandBase>>();
        }


        /// <summary>
        /// Append a batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="removeWhenDone"></param>
        /// <returns></returns>
        public void AppendCommand(object owner, ICommandBase cmd, bool removeWhenDone, int updateOrder = 0 , [CallerFilePath] string calledPath = "" , [CallerLineNumber] int lineNumber = 0)
        {
            Assert.IsNotNull(cmd);

#if DEBUG
            cmd.DebugInfo = new CommandDebugInfo()
            {
                AddedFromFilepath = calledPath,
                AddedFromLine = lineNumber,
            };
#endif

            cmd.UpdateOrder = updateOrder;
            cmd.Owner = owner;
            cmd.RemoveWhenDone = removeWhenDone;

            int layersDiff = updateOrder - (commandBatches.Count - 1);

            for (int i = 0; i < layersDiff; i++)
            {
                commandBatches.Add(new List<ICommandBase>());
            }

            commandBatches[updateOrder].Add(cmd);
        }

        public void Clear()
        {
            for (int l = 0; l < commandBatches.Count; l++)
            {
                for (int i = commandBatches[l].Count - 1; i > -1; i--)
                {
                    ICommandBase cmd = commandBatches[l][i];

                    cmd.Interrupt();
                }

                commandBatches[l].Clear();
            }

            commandBatches.Clear();
        }

        public void Tick(float deltaTime)
        {
            for (int l = 0; l < commandBatches.Count; l++)
            {
                List<ICommandBase> currCommands = commandBatches[l];

                bool cmdsWereAdded = true;

                int removeCount = 0;
                int startCount = currCommands.Count;
                int rangeStart = 0;

                while (cmdsWereAdded)
                {
                    removeCount = 0;
                    
                    for (int i = startCount - 1; i >= rangeStart; i--)
                    {
                        ICommandBase cmd = commandBatches[l][i];

                        if (cmd.CommandState == COMMAND_STATE.WATING)
                        {
                            cmd.Start();
                        }

                        if (cmd.IsDone() && cmd.RemoveWhenDone)
                        {
                            commandBatches[l].RemoveAt(i);
                            removeCount++;
                            continue;
                        }

                        // else tick
                        cmd.OnTick(deltaTime);
                    }

                    int leftOverCmds = startCount - removeCount;
                    cmdsWereAdded = currCommands.Count > leftOverCmds;

                    startCount = currCommands.Count;
                    rangeStart = leftOverCmds;
                }
            }
        }


    }
}
