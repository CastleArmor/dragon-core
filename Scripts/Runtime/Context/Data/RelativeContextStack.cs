using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    [CreateAssetMenu(fileName = "RCS_",menuName = "RelativeContextStack")]
    public class RelativeContextStack : ScriptableObject
    {
        [SerializeField] private List<DataKey> _contextKeys;
        public List<DataKey> ContextKeys => _contextKeys;
    }
}