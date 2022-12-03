#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandManager
    {

        #if ODIN_INSPECTOR[ShowInInspector]#endif
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
        public void AppendCommand(object owner, ICommandBase cmd, bool removeWhenDone, int updateOrder = 0)
        {
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

        public void Tick(float deltaTime)
        {
            for (int l = 0; l < commandBatches.Count; l++)
            {
                for (int i = commandBatches[l].Count - 1; i > -1; i--)
                {
                    ICommandBase cmd = commandBatches[l][i];

                    if (cmd.CommandState == COMMAND_STATE.WATING)
                    {
                        cmd.Start();
                    }

                    if (cmd.IsDone() && cmd.RemoveWhenDone)
                    {
                        commandBatches[l].RemoveAt(i);
                        continue;
                    }

                    // else tick
                    cmd.OnTick(deltaTime);
                }
            }
        }


    }
}
