namespace Bloodthirst.Editor.BRecorder
{
    public abstract class BRecorderActionBase
    {
        protected BRecorderEditor Recorder { get; private set; }

        public BRecorderActionBase(BRecorderEditor recorder)
        {
            Recorder = recorder;
        }

        public abstract void Initialize();

        public abstract void Destroy();
    }
}
