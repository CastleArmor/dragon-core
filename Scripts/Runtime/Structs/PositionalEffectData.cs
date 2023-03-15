using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable]
    public struct PositionalEffectData
    {
        [ShowInInspector]
        public Vector3 SourcePoint { get; set; }
        
        [ShowInInspector]
        public Collider SourceCollider { get; set; }
        
        [ShowInInspector]
        public Collider OtherCollider { get; set; }
        
        [ShowInInspector]
        public Vector3 Point { get; set; }
        
        [ShowInInspector]
        public Vector3 EffectDirection { get; set; }
    }
}