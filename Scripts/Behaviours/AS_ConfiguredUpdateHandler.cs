using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

    public interface IConfiguredUpdateObject : ICastable
    {
        void RegisterConfiguredUpdate(string tag,UpdateArgs args, Action action);
        void UnregisterConfiguredUpdate(string tag, Action action);
    }
    
    public interface IConfiguredUpdateBehaviour : IConfiguredUpdateObject
    {
        void InitializeIfNot();
        void TriggerUpdate(string tag);
    }
    public class AS_ConfiguredUpdateHandler : ActorService,IConfiguredUpdateBehaviour
    {
        public struct RemovalPair
        {
            public string Tag;
            public Action Action;
        }

        public enum UpdateType
        {
            Update,
            FixedUpdate,
            LateUpdate,
            Trigger
        }
        
        [SerializeField][FoldoutGroup("Update Config")]
        private List<UpdateKey> _fixedUpdateConfiguration = new List<UpdateKey>();
        [SerializeField][FoldoutGroup("Update Config")]
        private List<UpdateKey> _updateConfiguration = new List<UpdateKey>();
        [SerializeField][FoldoutGroup("Update Config")]
        private List<UpdateKey> _lateUpdateConfiguration = new List<UpdateKey>();
        [SerializeField][FoldoutGroup("Update Config")]
        private List<UpdateKey> _triggeredUpdateConfiguration = new List<UpdateKey>();
        [ShowInInspector][HideInEditorMode]
        private readonly Dictionary<string, List<Action>> _updateDictionary = new Dictionary<string, List<Action>>();
        [ShowInInspector][HideInEditorMode]
        private readonly Dictionary<string, List<Action>> _fixedUpdateDictionary = new Dictionary<string, List<Action>>();
        [ShowInInspector][HideInEditorMode]
        private readonly Dictionary<string, List<Action>> _lateUpdateDictionary = new Dictionary<string, List<Action>>();
        [ShowInInspector][HideInEditorMode]
        private readonly Dictionary<string, List<Action>> _triggeredUpdateDictionary = new Dictionary<string, List<Action>>();
        private readonly List<RemovalPair> _removalPairs = new List<RemovalPair>();
        private readonly List<RemovalPair> _additionPairs = new List<RemovalPair>();


        [ShowInInspector][ReadOnly]
        private bool _isInitialized;

        protected override void OnRegisterActor()
        {
            base.OnRegisterActor();
            
            DataRegistry<IConfiguredUpdateBehaviour>.SetData(Actor.DataContext,this);
            InitializeIfNot();
        }

        protected override void OnUnregisterMain()
        {
            base.OnUnregisterMain();
            _updateDictionary.Clear();
            _fixedUpdateDictionary.Clear();
            _lateUpdateDictionary.Clear();
            _triggeredUpdateDictionary.Clear();
        }

        public void InitializeIfNot()
        {
            if (_isInitialized) return;
            foreach (UpdateKey key in _updateConfiguration)
            {
                _updateDictionary.Add(key.ID,new List<Action>());
            }
            foreach (UpdateKey key in _fixedUpdateConfiguration)
            {
                _fixedUpdateDictionary.Add(key.ID,new List<Action>());
            }
            foreach (UpdateKey key in _lateUpdateConfiguration)
            {
                _lateUpdateDictionary.Add(key.ID,new List<Action>());
            }
            foreach (UpdateKey key in _triggeredUpdateConfiguration)
            {
                _triggeredUpdateDictionary.Add(key.ID,new List<Action>());
            }

            _isInitialized = true;
        }

        [Button]
        public void ApplyFromConfig(UpdateHandlerConfig config)
        {
            _updateConfiguration = new List<UpdateKey>(config.UpdateConfiguration);
            _fixedUpdateConfiguration = new List<UpdateKey>(config.FixedUpdateConfiguration);
            _lateUpdateConfiguration = new List<UpdateKey>(config.LateUpdateConfiguration);
        }

        [Button]
        public void SaveToConfig(UpdateHandlerConfig config)
        {
            config.UpdateConfiguration = new List<UpdateKey>(_updateConfiguration);
            config.FixedUpdateConfiguration = new List<UpdateKey>(_fixedUpdateConfiguration);
            config.LateUpdateConfiguration = new List<UpdateKey>(_lateUpdateConfiguration);
        }
        
        public void RegisterConfiguredUpdate(string updateTag, UpdateArgs args, Action action)
        {
            _additionPairs.Add(new RemovalPair(){Tag=updateTag,Action=action});
        }

        public void UnregisterConfiguredUpdate(string updateTag, Action action)
        {
            _removalPairs.Add(new RemovalPair(){Tag=updateTag,Action=action});
        }

        private void ExecuteConfiguredUpdateDictionary(Dictionary<string, List<Action>> dictionary)
        {
            foreach (List<Action> actions in dictionary.Values)
            {
                foreach (Action action in actions)
                {
                    action.Invoke();
                }
            }
        }

        private void ExecuteConfiguredUpdateRemovals(UpdateType updateType, bool specifiedTagOnly = false, string specifiedTag = "")
        {
            int index = 0;
            while (_removalPairs.Count > index)
            {
                string updateTag = _removalPairs[index].Tag;
                Action action = _removalPairs[index].Action;
                if (specifiedTagOnly)
                {
                    if (updateTag != specifiedTag)
                    {
                        index += 1;
                        continue;
                    }
                }
                if (_updateDictionary.ContainsKey(updateTag))
                {
                    if(updateType == UpdateType.Update)
                    {
                        _updateDictionary[updateTag].Remove(action);
                    }
                    else
                    {
                        index += 1;
                        continue;
                    }
                }
                if (_fixedUpdateDictionary.ContainsKey(updateTag))
                {
                    if (updateType == UpdateType.FixedUpdate)
                    {
                        _fixedUpdateDictionary[updateTag].Remove(action); 
                    }
                    else
                    {
                        index += 1;
                        continue;
                    }
                }
                if (_lateUpdateDictionary.ContainsKey(updateTag))
                {
                    if (updateType == UpdateType.LateUpdate)
                    {
                        _lateUpdateDictionary[updateTag].Remove(action);
                    }
                    else
                    {
                        index += 1;
                        continue;
                    }
                }
                if (_triggeredUpdateDictionary.ContainsKey(updateTag))
                {
                    if (updateType == UpdateType.Trigger)
                    {
                        _triggeredUpdateDictionary[updateTag].Remove(action);
                    }
                    else
                    {
                        index += 1;
                        continue;
                    }
                }
                _removalPairs.RemoveAt(index);
            }
        }

        private void ExecuteConfiguredUpdateAdditions(UpdateType updateType, bool specifiedTagOnly = false, string specifiedTag = "")
        {
            int index = 0;
            while (_additionPairs.Count > index)
            {
                string updateTag = _additionPairs[index].Tag;
                Action action = _additionPairs[index].Action;
                if (specifiedTagOnly)
                {
                    if (updateTag != specifiedTag)
                    {
                        index += 1;
                        continue;
                    }
                }
                if (_updateDictionary.ContainsKey(updateTag))
                {
                    if (updateType == UpdateType.Update)
                    {
                        _updateDictionary[updateTag].Add(action);
                    }
                    else
                    {
                        index += 1;
                        continue;
                    }
                }
                if (_fixedUpdateDictionary.ContainsKey(updateTag))
                {
                    if (updateType == UpdateType.FixedUpdate)
                    {
                        _fixedUpdateDictionary[updateTag].Add(action);  
                    }
                    else
                    {
                        index += 1;
                        continue;
                    } 
                }
                if (_lateUpdateDictionary.ContainsKey(updateTag))
                {
                    if (updateType == UpdateType.LateUpdate)
                    {
                        _lateUpdateDictionary[updateTag].Add(action);
                    }
                    else
                    {
                        index += 1;
                        continue;
                    }
                }
                if (_triggeredUpdateDictionary.ContainsKey(updateTag))
                {
                    if (updateType == UpdateType.Trigger)
                    {
                        _triggeredUpdateDictionary[updateTag].Add(action);
                    }
                    else
                    {
                        index += 1;
                        continue;
                    }
                }
                _additionPairs.RemoveAt(index);
            }
        }

        protected override void OnBeginBehaviour()
        {
            base.OnBeginBehaviour();
            StaticUpdate.onUpdate += OnUpdate;
            StaticUpdate.onFixedUpdate += OnFixedUpdate;
            StaticUpdate.onLateUpdate += OnLateUpdate;
        }

        protected override void OnUnregisterOrStopAfterBegin()
        {
            base.OnUnregisterOrStopAfterBegin();
            if (_isAppQuit) return;
            StaticUpdate.onUpdate -= OnUpdate;
            StaticUpdate.onFixedUpdate -= OnFixedUpdate;
            StaticUpdate.onLateUpdate -= OnLateUpdate;
        }

        private bool _isAppQuit;
        private void OnApplicationQuit()
        {
            _isAppQuit = true;
        }

        private void OnUpdate()
        {
            ExecuteConfiguredUpdateAdditions(UpdateType.Update);
            ExecuteConfiguredUpdateRemovals(UpdateType.Update);
            ExecuteConfiguredUpdateDictionary(_updateDictionary);
        }

        private void OnFixedUpdate()
        {
            ExecuteConfiguredUpdateAdditions(UpdateType.FixedUpdate);
            ExecuteConfiguredUpdateRemovals(UpdateType.FixedUpdate);
            ExecuteConfiguredUpdateDictionary(_fixedUpdateDictionary);
        }

        private void OnLateUpdate()
        {
            ExecuteConfiguredUpdateAdditions(UpdateType.LateUpdate);
            ExecuteConfiguredUpdateRemovals(UpdateType.LateUpdate);
            ExecuteConfiguredUpdateDictionary(_lateUpdateDictionary);
        }

        public void TriggerUpdate(string updateKey)
        {
            ExecuteConfiguredUpdateAdditions(UpdateType.Trigger,true,updateKey);
            ExecuteConfiguredUpdateRemovals(UpdateType.Trigger,true,updateKey);
            foreach (Action action in _triggeredUpdateDictionary[updateKey])
            {
                action.Invoke();
            }
        }
    }