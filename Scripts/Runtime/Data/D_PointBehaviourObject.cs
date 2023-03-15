using System;
using Sirenix.OdinInspector;

namespace Dragon.Core
{
    [System.Serializable]
    public class D_PointBehaviourObject : InstalledData
    {
        [ShowInInspector][ReadOnly]
        private bool _pointActivated;
        public event Action<IContext, bool, bool> onPointActivatedChanged;

        public bool PointActivated
        {
            get => _pointActivated;
            set
            {
                bool oldValue = _pointActivated;
                bool isChanged = _pointActivated != value;
                _pointActivated = value;
                if (isChanged)
                {
                    onPointActivatedChanged?.Invoke(Context, oldValue, value);
                }
            }
        }
    }
}