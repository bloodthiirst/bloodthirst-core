namespace Bloodthirst.System.CommandSystem
{
    public interface ICommandBatch
    {
        object Owner { get; set; }
        void Tick(float delta);

        void Interrupt();
    }
}