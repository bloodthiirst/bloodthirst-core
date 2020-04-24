namespace Bloodthirst.Core.ThreadProcessor
{
    public interface ISeparateThreadCommand
    {
        void Start();
        void ExecuteCallback();
        bool IsDone { get; }
    }
}
