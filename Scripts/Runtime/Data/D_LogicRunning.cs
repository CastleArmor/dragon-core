using System;
using Sirenix.OdinInspector;

namespace Dragon.Core
{
    public enum LogicStatus
    {
        Greet = 1,
        Running = 2,
        Stopped = 3,
        Ended = 4,
        Finalized = 5
    }

    [System.Serializable]
    public class D_LogicRunning
    {
        [ShowInInspector][ReadOnly]
        private LogicStatus _status;
        public LogicStatus Status => _status;

        [ShowInInspector][ReadOnly]
        private Key _outcome;
        public Key Outcome
        {
            get => _outcome;
            set => _outcome = value;
        }

        public void ChangeStatus(LogicStatus status)
        {
            LogicStatus old = _status;
            _status = status;
            if (old != _status)
            {
                onStatusChanged?.Invoke(this,old,_status);
            }
        }

        public event Action<D_LogicRunning,LogicStatus,LogicStatus> onStatusChanged; 
    }
}