namespace Bloodthirst.Editor.BRecorder
{
    public abstract class BRecorderActionBase
    {
        protected BRecorder Recorder { get; private set; }

        public BRecorderActionBase(BRecorder recorder)
        {
            Recorder = recorder;
        }

        public abstract void Initialize();

        public abstract void Destroy();
    }
}
