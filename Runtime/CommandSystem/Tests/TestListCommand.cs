using Bloodthirst.System.CommandSystem;


namespace Commands.Tests
{
    public class TestListCommand : ListCommandBase<TestListCommand>
    {
        public TestListCommand( CommandManager commandManager = null, bool failCommandIfInterrupted = false):base( commandManager, failCommandIfInterrupted)
        {

        }
    }
}
