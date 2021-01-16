using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public enum COMMAND_STATE
    {
        WATING, EXECUTING, SUCCESS, FAILED, INTERRUPTED
    }
    public abstract class CommandBase<T> : ICommandBase where T : CommandBase<T>
    {
#if UNITY_EDITOR
        public bool detailedInfo;
#endif
        public event Action<ICommandBase> OnCommandStart;

        public event Action<ICommandBase> OnCommandEnd;

        public event Action<T> OnCommandEndSpecific;

        [SerializeField]
#if UNITY_EDITOR
        [ShowIf(nameof(detailedInfo), Value = true)]
#endif
        private COMMAND_STATE commandState;
        public COMMAND_STATE CommandState { get => commandState; set => commandState = value; }

        [SerializeField]
#if UNITY_EDITOR
        [ShowIf(nameof(detailedInfo), Value = true)]
#endif
        private ICommandBase fallbackCommand;

        public ICommandBase FallbackCommand { get => fallbackCommand; set => fallbackCommand = value; }

        [SerializeField]
#if UNITY_EDITOR
        [ShowIf(nameof(detailedInfo), Value = true)]
#endif
        private bool isDone;
        public bool IsDone { get => isDone; }

        [SerializeField]
#if UNITY_EDITOR
        [ShowIf(nameof(detailedInfo), Value = true)]
#endif
        private bool isStarted;
        public bool IsStarted => isStarted;

        /// <summary>
        /// Executed on command start , defined in command and used by ICommandBatch
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// Executed on every frame of the commands lifetime , defined in command and used by ICommandBatch
        /// </summary>
        public virtual void OnTick(float delta) { }

        /// <summary>
        /// Executed when the command ends with the status of success , defined in command and triggered automatically
        /// </summary>
        public virtual void OnSuccess() { }

        /// <summary>
        /// Executed when the command ends with the status of failed , defined in command and triggered automatically
        /// </summary>
        public virtual void OnFailed() { }

        /// <summary>
        /// Executed when the command ends with the status of interrupted , defined in command and triggered automatically
        /// </summary>
        public virtual void OnInterrupt() { }

        /// <summary>
        /// Executed when the command ends regardless of the status , defined in command and triggered automatically
        /// </summary>
        public virtual void OnEnd() { }

        public CommandBase()
        {
            isStarted = false;
            isDone = false;
            CommandState = COMMAND_STATE.WATING;
            FallbackCommand = null;
        }

        public ICommandBase SetFallbackCommand(ICommandBase fallback)
        {
            FallbackCommand = fallback;
            return this;
        }
        public ICommandBase GetExcutingCommand()
        {
            // waiting
            switch (CommandState)
            {
                case COMMAND_STATE.WATING:
                    return this;
                case COMMAND_STATE.EXECUTING:
                    return this;
                case COMMAND_STATE.SUCCESS:
                    return this;
                case COMMAND_STATE.INTERRUPTED:
                    return this;
                /// in this case default is COMMAND_STATE.FAILED
                default:
                    {
                        if (FallbackCommand == null)
                            return this;

                        // propagate to the fallback command
                        return FallbackCommand.GetExcutingCommand();
                    }
            }
        }

        /// <summary>
        /// Starts the command , called by ICommandBatch instances
        /// </summary>
        public void Start()
        {
            CommandState = COMMAND_STATE.EXECUTING;
            isStarted = true;
            OnStart();
            OnCommandStart?.Invoke(this);
        }

        /// <summary>
        /// Ends the command , called by automatically (interal)
        /// </summary>
        private void End()
        {
            isDone = true;
            OnEnd();
            OnCommandEnd?.Invoke(this);
            OnCommandEndSpecific?.Invoke((T)this);
        }

        #region method to use from within the command to control its lifetime

        /// <summary>
        /// Ends the command with status of SUCCESS
        /// </summary>
        public void Success()
        {
            CommandState = COMMAND_STATE.SUCCESS;
            OnSuccess();
            End();
        }

        /// <summary>
        /// Ends the command with status of FAILED
        /// </summary>
        public void Fail()
        {
            CommandState = COMMAND_STATE.FAILED;
            OnFailed();
            End();
        }

        /// <summary>
        /// Ends the command with status of INTERRUPTED
        /// </summary>
        public void Interrupt()
        {
            // if is done
            // then there's nothing to interrupt
            if (isDone)
            {
                return;
            }
            // if not started
            // then just set it to done 
            // no need to trigger other events
            if (!IsStarted)
            {
                isDone = true;
                return;
            }

            OnInterrupt();

            CommandState = COMMAND_STATE.INTERRUPTED;

            End();
        }

        /// <summary>
        /// Ends the command with NO status and reset it to start over
        /// </summary>
        public void Reset()
        {
            // end first to trigger the events if some other command need them
            End();

            // reset the parameters
            isDone = false;
            isStarted = false;
            CommandState = COMMAND_STATE.EXECUTING;
        }
        #endregion

    }
}
