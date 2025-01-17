﻿using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class OnTimelineMouseMove : BRecorderEventBase
    {
        public MouseMoveEvent MouseMoveEvent { get; }

        public OnTimelineMouseMove(BRecorderEditor recorder, MouseMoveEvent MouseMoveEvent) : base(recorder)
        {
            this.MouseMoveEvent = MouseMoveEvent;
        }
    }
}
