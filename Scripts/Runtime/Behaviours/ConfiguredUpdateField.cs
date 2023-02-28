using System;
using System.Collections;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

[System.Serializable][TopTitle(SetName = true,SetParentObject = true)]
    public struct ConfiguredUpdateField
    {
        [SerializeField] [HideInInspector] private string _name;
        public string name
        {
            get => _name;
            set => _name = value;
        }
        [SerializeField] [HideInInspector] private Object _parentObject;
        public Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }
        [HideLabel]
        [SerializeField] private UpdateKey _updateKey;

        public UpdateKey UpdateKey => _updateKey;

        [SerializeField][HorizontalGroup][HideIf("_useParent")]private string _updateName;
        [SerializeField][HorizontalGroup][HideLabel]  private bool _useParent;
        [ShowInInspector][ReadOnly][HorizontalGroup][HideLabel][ShowIf("_useParent")] private string UpdateName=> _useParent&&parentObject!=null?parentObject.GetType().Name + name:_updateName;

        private bool _isRegistered;

        public void RegisterUpdate(IActor actor,Action action)
        {
            if (_isRegistered) return;
            actor.DataContext.GetData<IConfiguredUpdateBehaviour>().RegisterConfiguredUpdate(_updateKey.ID,new UpdateArgs()
            {
                UpdateName = UpdateName
            }, action);
            _isRegistered = true;
        }

        public void RegisterUpdate(IConfiguredUpdateObject stateComponent, Action action)
        {
            if (_isRegistered) return;
            stateComponent.RegisterConfiguredUpdate(_updateKey.ID, new UpdateArgs()
            {
                UpdateName = UpdateName
            }, action);
            _isRegistered = true;
        }

        public void UnregisterUpdate(IConfiguredUpdateObject stateComponent, Action action)
        {
            if (!_isRegistered) return;
            stateComponent.UnregisterConfiguredUpdate(_updateKey.ID, action);
            _isRegistered = false;
        }
        
        public void UnregisterUpdate(IActor actor, Action action)
        {
            if (!_isRegistered) return;
            actor.DataContext.GetData<IConfiguredUpdateBehaviour>().UnregisterConfiguredUpdate(_updateKey.ID, action);
            _isRegistered = false;
        }
    }