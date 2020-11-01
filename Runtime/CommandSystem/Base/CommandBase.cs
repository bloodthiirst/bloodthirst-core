using Sirenix.OdinInspector;
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
#if UNITY_EDITOR
        public bool detailedInfo;
#endif
        public event Action OnCommandStart;

        public event Action OnCommandEnd;

        public event Action<ICommandBase> OnSpecificCommandEnd;

        [SerializeField]
#if UNITY_EDITOR
        [ShowIf( nameof(detailedInfo) , Value = true )]
#endif
        private COMMAND_STATE commandState;
        public COMMAND_STATE CommandState { get => commandState; set => commandState = value; }

        [SerializeField]
#if UNITY_EDITOR
        [ShowIf(nameof(detailedInfo), Value = true)]
#endif
        private List<ICommandBase> fallbackCommands;

        public List<ICommandBase> FallbackCommands { get => fallbackCommands; set => fallbackCommands = value; }

        [SerializeField]
#if UNITY_EDITOR
        [ShowIf(nameof(detailedInfo), Value = true)]
#endif
        private bool isDone;
        public bool IsDone { get => isDone; set => isDone = value; }

        [SerializeField]
#if UNITY_EDITOR
        [ShowIf(nameof(detailedInfo), Value = true)]
#endif
        private bool isStarted;
        public bool IsStarted { get => isStarted; set => isStarted = value; }

        public virtual void OnStart() { }
        
        public virtual void OnTick(float delta) { }

        public virtual void OnEnd() { }

        public virtual void OnInterrupt() { }

        public CommandBase()
        {
            IsStarted = false;
            IsDone = false;
            CommandState = COMMAND_STATE.EXECUTING;
            FallbackCommands = new List<ICommandBase>();
        }
        public void OnCommandStartNotify()
        {
            OnCommandStart?.Invoke();
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
            OnEnd();
            IsDone = true;
            CommandState = COMMAND_STATE.SUCCESS;

            OnCommandEnd?.Invoke();
            OnSpecificCommandEnd?.Invoke(this);
        }
    }
}
