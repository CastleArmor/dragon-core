using System;

namespace Dragon.Core
{
    [System.Serializable]
    public class InstalledData : IInstalledData
    {
        [NonSerialized]
        private IContext _context;
        public IContext Context => _context;
    
        [NonSerialized]
        private IDataContext _dataContext;
        public IDataContext DataContext => _dataContext;
    
        [NonSerialized]
        private IEventContext _eventContext;
        public IEventContext EventContext => _eventContext;
    
        [NonSerialized]
        private bool _isInstalled;
        public bool IsInstalled => _isInstalled;

        [NonSerialized]
        private bool _isInitializing;
        public bool IsInitializing => _isInitializing;

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

        public void OnInstalledData(IContext context)
        {
            _context = context;
            if (_context != null)
            {
                _dataContext = context as IDataContext;
                _eventContext = context as IEventContext;
            }
            _isInitializing = true;
            OnBindAdditional(context);
            OnInitialize();
            if (_dataContext != null)
            {
                if (!_dataContext.IsPrefab && !_dataContext.IsDefaultPrefabInstance)
                {
                    OnInitializeInstanceData();
                }
            }

            _isInitializing = false;
            _isInstalled = true;
        }

        protected virtual void OnBindAdditional(IContext context)
        {
        
        }
    
        protected virtual void OnInitialize()
        {
        
        }

        protected virtual void OnInitializeInstanceData()
        {
        
        }

        public void OnRemoveData()
        {
            if (_dataContext != null)
            {
                if (!_dataContext.IsPrefab && !_dataContext.IsDefaultPrefabInstance)
                {
                    OnInitializeInstanceData();
                }
            }

            OnRemove();
            _isInstalled = false;
        }
    
        protected virtual void OnRemove()
        {
        
        }
    }
}