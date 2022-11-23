namespace Bloodthirst.System.CommandSystem
{
    public class BasicQueueCommand : QueueCommandBase<BasicQueueCommand>
    {
        /// <summary>
        /// <para>Setup the settings of the QueueCommand</para>
        /// <para>Inject a <see cref="CommandManager"/> through <paramref name="commandManager"/> to be used for the sub command , if no command manager is passed then the <see cref="CommandManagerBehaviour"/> singleton will be used as the command maanger</para>
        /// <para>The <paramref name="failCommandIfInterrupted"/> determines if the command should mark its state as <see cref="COMMAND_STATE.FAILED"/> if its internal <see cref="CommandBatchList"/> gets the state of <see cref="BATCH_STATE.INTERRUPTED"/> </para>
        /// </summary>
        /// <param name="commandManager"></param>
        /// <param name="failCommandIfInterrupted"></param>
        public BasicQueueCommand( bool propagateFailOrInterrupt) : base(propagateFailOrInterrupt)
        {

        }
    }
}
