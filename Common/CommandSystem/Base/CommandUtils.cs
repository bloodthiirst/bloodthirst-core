namespace Bloodthirst.System.CommandSystem
{
    public static class CommandUtils
    {
        public static bool IsDone(this ICommandBase command)
        {
            return 
                command.CommandState != COMMAND_STATE.WATING && 
                command.CommandState != COMMAND_STATE.EXECUTING;
        }
    }
}
