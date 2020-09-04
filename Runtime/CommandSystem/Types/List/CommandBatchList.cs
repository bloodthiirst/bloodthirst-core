﻿using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public class CommandBatchList : ICommandBatch
    {
        [SerializeField]
        private List<ICommandBase> commandList;
        public List<ICommandBase> CommandsList { get => commandList; set => commandList = value; }

        [SerializeField]
        private object owner;
        public object Owner { get => owner; set => owner = value; }


        [SerializeField]
        private bool removeWhenDone;
        public bool RemoveWhenDone { get => removeWhenDone; set => removeWhenDone = value; }

        [SerializeField]
        private BATCH_STATE batchState;
        public BATCH_STATE BatchState { get => batchState; set => batchState = value; }

        List<ICommandBase> removeCache = new List<ICommandBase>();

        public CommandBatchList()
        {
            CommandsList = new List<ICommandBase>();
            BatchState = BATCH_STATE.EXECUTING;
        }
        public void Tick(float delta)
        {
            // clear the remove cache

            removeCache.Clear();

            // register the removable commands

            for (int i = 0; i < CommandsList.Count; i++)
            {
                if (CommandsList[i].GetExcutingCommand().IsDone)
                {
                    removeCache.Add(CommandsList[i]);

                    CommandsList.RemoveAt(i);
                }
            }

            // remove the ended commands

            for (int i = 0; i < removeCache.Count; i++)
            {
                removeCache[i].OnEnd();
                CommandsList.Remove(removeCache[i]);
            }

            // execute the commands alive and execute tick and start

            for (int i = 0; i < CommandsList.Count; i++)
            {
                // if command is not started , execute the command start
                if (!CommandsList[i].IsStarted)
                {
                    CommandsList[i].GetExcutingCommand().OnStart();
                    CommandsList[i].GetExcutingCommand().IsStarted = true;
                    CommandsList[i].GetExcutingCommand().OnCommandStartNotify();
                }

                // execute the commands on tick
                CommandsList[i].GetExcutingCommand().OnTick(delta);
            }

            // state depends on commands pending
            BatchState = CommandsList.Count == 0 ? BATCH_STATE.DONE : BATCH_STATE.EXECUTING;
        }

        public ICommandBatch Append(ICommandBase command)
        {
            CommandsList.Add(command);
            return this;
        }

        public void Interrupt()
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                if (commandList[i].CommandState == COMMAND_STATE.EXECUTING)
                {
                    commandList[i].Interrupt();
                }
            }

            BatchState = BATCH_STATE.DONE;
        }
    }
}
