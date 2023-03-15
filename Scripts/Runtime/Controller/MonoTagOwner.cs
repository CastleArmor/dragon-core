using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    public class MonoTagOwner : MonoBehaviour,ITagOwner,IUnityComponent,IInitializable
    {
        [SerializeField][HideInPlayMode] private List<Key> _tags;
        private readonly HashSet<string> _tagSet = new HashSet<string>();
        [ShowInInspector][ReadOnly][HideInEditorMode]
        private readonly List<string> _tagList = new List<string>();
        public List<string> TagList => _tagList;

        public event Action<ITagOwner, string> onTagAdded;
        public event Action<ITagOwner, string> onTagRemoved;
        /// <summary>
        /// Last bool returns true if addition, and false if removal.
        /// </summary>
        public event Action<ITagOwner, string, bool> onTagChanged;
        public bool ContainsTag(string t)
        {
            return _tagSet.Contains(t);
        }

        public void AddTag(string t)
        {
            _tagSet.Add(t);
            _tagList.Add(t);
            onTagAdded?.Invoke(this,t);
            onTagChanged?.Invoke(this,t,true);
        }

        public void RemoveTag(string t)
        {
            _tagSet.Remove(t);
            _tagList.Remove(t);
            onTagRemoved?.Invoke(this,t);
            onTagChanged?.Invoke(this,t,false);
        }
    
        [ShowInInspector][ReadOnly]
        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;
        public void InitializeIfNot()
        {
            if (_isInitialized) return;
            for (var i = 0; i < _tags.Count; i++)
            {
                var t = _tags[i].ID;
                _tagList.Add(t);
                _tagSet.Add(t);
            }
            _isInitialized = true;
        }
    }
}