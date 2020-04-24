using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public enum COMMAND_STATE
    {
        EXECUTING, SUCCESS, FAILED
    }
    public abstract class CommandBase<T> : ICommandBase where T : CommandBase<T>
    {

        public event Action OnCommandStart;

        public event Action OnCommandEnd;

        public event Action<ICommandBase> OnSpecificCommandEnd;

        [SerializeField]
        private COMMAND_STATE commandState;
        public COMMAND_STATE CommandState { get => commandState; set => commandState = value; }

        [SerializeField]
        private List<ICommandBase> fallbackCommands;
        public List<ICommandBase> FallbackCommands { get => fallbackCommands; set => fallbackCommands = value; }

        [SerializeField]
        private bool isDone;
        public bool IsDone { get => isDone; set => isDone = value; }

        [SerializeField]
        private bool isStarted;
        public bool IsStarted { get => isStarted; set => isStarted = value; }

        [SerializeField]
        private bool remove;
        public bool Remove { get => remove; set => remove = value; }

        public abstract void OnStart();
        public abstract void OnTick(float delta);
        public abstract void OnEnd();

        public virtual void OnInterrupt() { }

        public CommandBase()
        {
            IsStarted = false;
            IsDone = false;
            CommandState = COMMAND_STATE.EXECUTING;
            FallbackCommands = new List<ICommandBase>();
        }

        public ICommandBase AddFallback(ICommandBase fallback)
        {
            FallbackCommands.Add(fallback);
            return this;
        }
        public ICommandBase GetExcutingCommand()
        {
            // executing 
            if (CommandState == COMMAND_STATE.EXECUTING)
            {
                return this;
            }

            // success
            if (CommandState == COMMAND_STATE.SUCCESS)
            {
                return this;
            }

            // failed

            if (FallbackCommands.Count == 0)
                return this;

            return FallbackCommands[0].GetExcutingCommand();

        }
        public void Success()
        {
            IsDone = true;
            CommandState = COMMAND_STATE.SUCCESS;

            OnCommandEnd?.Invoke();
            OnSpecificCommandEnd?.Invoke(this);
        }

        public void Failed()
        {
            IsDone = true;
            CommandState = COMMAND_STATE.FAILED;

            OnCommandEnd?.Invoke();
            OnSpecificCommandEnd?.Invoke(this);
        }

        public void Reset()
        {
            IsDone = false;
            IsStarted = false;
            CommandState = COMMAND_STATE.EXECUTING;

            OnCommandEnd?.Invoke();
            OnSpecificCommandEnd?.Invoke(this);
        }

        public void Interrupt()
        {
            OnInterrupt();
            IsDone = true;
            CommandState = COMMAND_STATE.SUCCESS;

            OnCommandEnd?.Invoke();
            OnSpecificCommandEnd?.Invoke(this);
        }
    }
}
