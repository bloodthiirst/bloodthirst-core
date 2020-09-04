namespace Bloodthirst.System.CommandSystem
{
    public enum BATCH_STATE
    {
        EXECUTING, DONE
    }
    public interface ICommandBatch
    {
        BATCH_STATE BatchState { get; set; }
        object Owner { get; set; }
        void Tick(float delta);
        bool RemoveWhenDone { get; set; }
        void Interrupt();
    }
}