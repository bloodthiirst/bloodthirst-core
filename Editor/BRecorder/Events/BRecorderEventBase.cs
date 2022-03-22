namespace Bloodthirst.Editor.BRecorder
{
    public abstract class BRecorderEventBase
    {
        public BRecorderEditor Recorder { get; private set; }

        public BRecorderEventBase(BRecorderEditor recorder)
        {
            Recorder = recorder;
        }
    }
}
