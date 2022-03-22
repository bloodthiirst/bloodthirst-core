using Bloodthirst.Runtime.BRecorder;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineSessionChanged : BRecorderEventBase
    {
        public BRecorderSession OldSession { get; }
        public BRecorderSession NewSession { get; }

        public OnTimelineSessionChanged(BRecorderEditor recorder, BRecorderSession OldSession, BRecorderSession NewSession) : base(recorder)
        {
            this.OldSession = OldSession;
            this.NewSession = NewSession;
        }
    }
}
