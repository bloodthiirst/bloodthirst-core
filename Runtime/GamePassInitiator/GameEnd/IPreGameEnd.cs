namespace Bloodthirst.Core.Setup
{
    public interface IPreGameEnd
    {
        int Order { get; }
        void Execute();
    }
}
