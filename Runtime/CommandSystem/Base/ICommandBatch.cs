using System;

namespace Bloodthirst.System.CommandSystem
{
    public enum BATCH_STATE
    {
        EXECUTING, DONE , INTERRUPTED
    }
    public interface ICommandBatch
    {
        event Action<ICommandBatch> OnBatchEnded;
        event Action<ICommandBatch, ICommandBase> OnCommandRemoved;
        event Action<ICommandBatch, ICommandBase> OnCommandAdded;
        BATCH_STATE BatchState { get; set; }
        object Owner { get; set; }
        void Tick(float delta);
        bool RemoveWhenDone { get; set; }
        void Interrupt();
        bool ShouldRemove();
        void End();
    }
}