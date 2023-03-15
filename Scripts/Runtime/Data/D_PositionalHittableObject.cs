using System;
using Sirenix.OdinInspector;

namespace Dragon.Core
{
    [System.Serializable]
    public class D_PositionalHittableObject : InstalledData
    {
        [ShowInInspector][ReadOnly]
        private bool _hasIncomingHit;
        public event Action<IContext, bool, bool> onHasIncomingAttackChanged;
        public bool HasIncomingHit
        {
            get => _hasIncomingHit;
            set
            {
                bool oldValue = _hasIncomingHit;
                bool isChanged = _hasIncomingHit != value;
                _hasIncomingHit = value;
                if (isChanged)
                {
                    onHasIncomingAttackChanged?.Invoke(Context, oldValue, value);
                }
            }
        }
    
        [ShowInInspector][ReadOnly]
        private PositionalEffectData _latestIncomingHit;
        public event Action<IContext, PositionalEffectData, PositionalEffectData> onLatestIncomingHitDataChanged;

        public PositionalEffectData LatestIncomingHit
        {
            get => _latestIncomingHit;
            set
            {
                PositionalEffectData oldValue = _latestIncomingHit;
                _latestIncomingHit = value;
                onLatestHitDataChanged?.Invoke(Context, oldValue, value);
            }
        }
    
        [ShowInInspector][ReadOnly]
        private PositionalEffectData _latestHitData;
        public event Action<IContext, PositionalEffectData, PositionalEffectData> onLatestHitDataChanged;

        public PositionalEffectData LatestHitData
        {
            get => _latestHitData;
            set
            {
                PositionalEffectData oldValue = _latestHitData;
                _latestHitData = value;
                onLatestHitDataChanged?.Invoke(Context, oldValue, value);
            }
        }
    }
}