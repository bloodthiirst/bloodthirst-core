namespace Bloodthirst.Editor.BRecorder
{
    public abstract class BRecorderEventBase
    {
        public BRecorder Recorder { get; private set; }

        public BRecorderEventBase(BRecorder recorder)
        {
            Recorder = recorder;
        }
    }
}
