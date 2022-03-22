﻿namespace Bloodthirst.Runtime.BRecorder
{
    public interface IBRecorderCommand
    {
        float GameTime { get; }
        void Setup();
        void Execute();
    }
}
