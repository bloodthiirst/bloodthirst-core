namespace Bloodthirst.Core.Setup
{
    public interface IPreGameSetup
    {
        int Order { get; }
        void Execute();
    }
}
