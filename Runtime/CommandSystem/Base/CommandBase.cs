﻿using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.System.CommandSystem
{
    public enum COMMAND_STATE
    {
        WATING, EXECUTING, SUCCESS, FAILED, INTERRUPTED
    }
    public abstract class CommandBase<T> : ICommandBase where T : CommandBase<T>
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [HideInEditorMode]
#endif
#if UNITY_EDITOR
        [SerializeField]
        private bool detailedInfo;
#endif
        public event Action<ICommandBase> OnCommandStart;

        public event Action<ICommandBase> OnCommandEnd;

        public event Action<T> OnCommandEndSpecific;

        [SerializeField]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [ShowIf(nameof(detailedInfo), Value = true)]
        [HideInEditorMode]
#endif


        private COMMAND_STATE commandState;
        public COMMAND_STATE CommandState { get => commandState; set => commandState = value; }

        [SerializeField]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [ShowIf(nameof(detailedInfo), Value = true)]
        [HideInEditorMode]
#endif
        private ICommandBase fallbackCommand;

        public ICommandBase FallbackCommand { get => fallbackCommand; set => fallbackCommand = value; }

        [SerializeField]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [ShowIf(nameof(detailedInfo), Value = true)]
        [HideInEditorMode]
#endif
        private object owner;

        public object Owner { get => owner; set => owner = value; }

        [SerializeField]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [ShowIf(nameof(detailedInfo), Value = true)]
        [HideInEditorMode]
#endif
        private bool removeWhenDone;

        public bool RemoveWhenDone { get => removeWhenDone; set => removeWhenDone = value; }

        [SerializeField]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [ShowIf(nameof(detailedInfo), Value = true)]
        [HideInEditorMode]
#endif

        private int updateOrder;
        public int UpdateOrder { get => updateOrder; set => updateOrder = value; }


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
            CommandState = COMMAND_STATE.WATING;
            FallbackCommand = null;
        }

        public ICommandBase SetFallbackCommand(ICommandBase fallback)
        {
            FallbackCommand = fallback;
            return this;
        }
        public virtual ICommandBase GetExcutingCommand()
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
            OnStart();
            OnCommandStart?.Invoke(this);
        }

        /// <summary>
        /// Ends the command , called by automatically (interal)
        /// </summary>
        private void End()
        {
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
            // if is done or has started
            // then there's nothing to interrupt
            if (CommandState != COMMAND_STATE.EXECUTING)
            {
                CommandState = COMMAND_STATE.INTERRUPTED;
                return;
            }

            CommandState = COMMAND_STATE.INTERRUPTED;
            OnInterrupt();
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
            CommandState = COMMAND_STATE.EXECUTING;
        }
        #endregion

    }
    public abstract class CommandBase<T, TResult> : CommandBase<T>, IResult<TResult> where T : CommandBase<T, TResult>
    {
        public bool IsReady => CommandState == COMMAND_STATE.SUCCESS;
        public TResult Result { get; protected set; }


    }
}
