namespace Bloodthirst.System.CommandSystem
{
    public struct CommandSettings
    {
        public ICommandBase Command { get; set; }
        public bool InterruptBatchOnFail { get; set; }
    }
}
