using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    public class UpdateHandlerConfig : ScriptableObject
    {
        [SerializeField][FoldoutGroup("Update Config")]
        private List<UpdateKey> _updateConfiguration = new List<UpdateKey>();
        public List<UpdateKey> UpdateConfiguration
        {
            get => _updateConfiguration;
            set => _updateConfiguration = value;
        }

        [SerializeField][FoldoutGroup("Update Config")]
        private List<UpdateKey> _fixedUpdateConfiguration = new List<UpdateKey>();
        public List<UpdateKey> FixedUpdateConfiguration
        {
            get => _fixedUpdateConfiguration;
            set => _fixedUpdateConfiguration = value;
        }

        [SerializeField][FoldoutGroup("Update Config")]
        private List<UpdateKey> _lateUpdateConfiguration = new List<UpdateKey>();
        public List<UpdateKey> LateUpdateConfiguration
        {
            get => _lateUpdateConfiguration;
            set => _lateUpdateConfiguration = value;
        }
    }
}