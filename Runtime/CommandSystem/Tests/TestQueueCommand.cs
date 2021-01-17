using Bloodthirst.System.CommandSystem;


namespace Commands.Tests
{
    public class TestQueueCommand : QueueCommandBase<TestQueueCommand>
    {
        public TestQueueCommand( CommandManager commandManager = null, bool failCommandIfInterrupted = false):base( commandManager, failCommandIfInterrupted)
        {

        }
    }
}
