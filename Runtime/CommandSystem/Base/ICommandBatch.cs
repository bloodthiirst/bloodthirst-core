namespace Bloodthirst.System.CommandSystem
{
    public interface ICommandBatch
    {
        object Owner { get; set; }
        ICommandBatch Append(ICommandBase command);
        void Tick(float delta);

        void Interrupt();
    }
}