using System;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable]
    public class ContextData : IContextData
    {
        [NonSerialized]
        private IContext _context;
        public IContext pContext => _context;
    
        [NonSerialized]
        private bool _isInstalled;
        public bool IsInstalled => _isInstalled;

        [NonSerialized]
        private bool _isInitializing;
        public bool IsInitializing => _isInitializing;

        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;

        [NonSerialized]
        private string _assignedID;
        public string AssignedID
        {
            get => _assignedID;
            set => _assignedID = value;
        }

        [NonSerialized]
        private string _keyID;
        public string KeyID
        {
            get => _keyID;
            set => _keyID = value;
        }

        public void SetInstallParameters(IContext context, string key, string assignedID)
        {
            _context = context;
            _assignedID = assignedID;
            _keyID = key;
            OnAssignedContext();
            _isInstalled = true;
        }
        
        protected virtual void OnAssignedContext(){}

        protected virtual void OnBindAdditional(IContext context)
        {
        
        }

        public void InitializeIfNot()
        {
            if (_isInitializing || _isInitialized) return;
            
            _isInitializing = true;
            OnInitialize();
            if (_context != null)
            {
                if (!_context.IsPrefab && !_context.IsDefaultPrefabInstance)
                {
                    OnInitializeInstanceData();
                }
            }

            _isInitializing = false;
            _isInitialized = true;
        }
    
        protected virtual void OnInitialize()
        {
        
        }

        protected virtual void OnInitializeInstanceData()
        {
        
        }
    
        protected virtual void OnRemove()
        {
        
        }

        public void OnToggleBinding(IContext context, string key, string assignedID)
        {
            OnBindAdditional(context);
        }

        public void FinalizeIfNot()
        {
            if (!_isInitialized) return;
            if (_context != null)
            {
                if (!_context.IsPrefab && !_context.IsDefaultPrefabInstance)
                {
                    OnInitializeInstanceData();
                }
            }

            OnRemove();
            _isInitialized = false;
            _isInstalled = false;
        }
    }
}