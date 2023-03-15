using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    [DisallowMultipleComponent]
    public class GOInstance : MonoBehaviour, IGOInstance
    {
        [SerializeField][InlineButton("SetSelfName")] private string _objectTypeID;
        public string ObjectTypeID => _objectTypeID;
        public void SetSelfName()
        {
            _objectTypeID = gameObject.name;
        }

        //Used by odin.
        [SerializeField] private bool _showOptions;

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (string.IsNullOrEmpty(_objectTypeID))
                {
                    _objectTypeID = gameObject.name;
                }
            }
        }

        [SerializeField][FoldoutGroup("Pool Optional Settings")][ShowIf("_showOptions")] private int _defaultInitialCount = 0;
        public int DefaultInitialCount =>_defaultInitialCount;
        private bool _isDefaultPrefabInstance;
        public bool IsDefaultPrefabInstance
        {
            get => _isDefaultPrefabInstance;
            set => _isDefaultPrefabInstance = value;
        }
    
        private bool _isBeingDestroyed;
        public bool IsBeingDestroyed => _isBeingDestroyed;

        private bool _isPooledInstance;
        public bool IsPooledInstance => _isPooledInstance;

        private bool _isRetrievedFromPool;
        public bool IsRetrievedFromPool => _isRetrievedFromPool;

        private IGOInstancePool _pool;

        [FoldoutGroup("Settings")]
        [SerializeField][ShowIf("_showOptions")] private bool _deactivateOnReturn = true;
        [FoldoutGroup("Settings")]
        [SerializeField][ShowIf("_showOptions")] private bool _activateOnRetrieve = true;
        [FoldoutGroup("Settings")]
        [SerializeField][ShowIf("_showOptions")] private bool _setActiveFalseIfNotRegistered = true;
        [ShowInInspector][ReadOnly][HideReferenceObjectPicker][ShowIf("_showOptions")]
        GameObject Original { get; }
    
        public event Action onReturnPool;
        public event Action onRetrieved;

        public void PoolCheckoutRegisteredToPool(IGOInstancePool pool)
        {
            _pool = pool;
            _isPooledInstance = true;
        }

        public void PoolCheckoutRetrievedFromPool()
        {
            _isRetrievedFromPool = true;
            OnRetrieved();
            onRetrieved?.Invoke();
        }

        private void OnRetrieved()
        {
            if (_activateOnRetrieve)
            {
                gameObject.SetActive(true);
            }
        }

        public void PoolCheckoutReturnedToPool()
        {
            _isRetrievedFromPool = false;
            OnReturnToPool();
            onReturnPool?.Invoke();
        }

        private void OnReturnToPool()
        {
            if (_deactivateOnReturn)
            {
                gameObject.SetActive(false);
            }
        }

        public void ReturnPool()
        {
            if (!_isPooledInstance)
            {
                if (_setActiveFalseIfNotRegistered)
                {
                    gameObject.SetActive(false);
                }
                return;
            }
            if (_isRetrievedFromPool)
            {
                if (_isBeingDestroyed)
                {
                    return;
                }
            
                _pool.Return(this);
            }
        }

        private void OnDestroy()
        {
            _isBeingDestroyed = true;
            if (!_isPooledInstance) return;
            if (_isRetrievedFromPool)
            {
                _pool.Return(this);
                _pool.UnregisterInstance(this);
            }
        }
    }
}