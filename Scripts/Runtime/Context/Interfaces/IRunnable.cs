﻿namespace Dragon.Core
{
    public interface IRunnable
    {
        bool IsRunning { get; }
        void BeginIfNot();
        void StopIfNot();
    }
}