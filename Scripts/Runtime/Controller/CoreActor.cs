using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    public class CoreActor : Actor,ITagOwner
    {
        [SerializeField] private DataInstaller<D_Positioning> _positioning;
        [SerializeField][ReadOnly] private MonoBehaviour _tagOwnerObject;
        private ITagOwner _tagOwner;
        [SerializeField] private MonoActorState _runningState;

        public IConfiguredUpdateBehaviour ConfiguredUpdateHandler { get; set; }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (Application.isPlaying) return;
            if (_tagOwnerObject != null && _tagOwnerObject != this) return;
            foreach (ITagOwner owner in GetComponents<ITagOwner>())
            {
                if (owner == (ITagOwner) this) continue;
                _tagOwnerObject = owner as MonoBehaviour;
            }

            if (_tagOwnerObject == this)
            {
                _tagOwnerObject = null;
            }
        }

        protected override void OnBeforeContextsInitialize()
        {
            base.OnBeforeContextsInitialize();
            if (_tagOwnerObject)
            {
                _tagOwner = _tagOwnerObject.As<ITagOwner>();
                _tagOwner.As<IInitializable>().InitializeIfNot();
            }
        }

        protected override void OnAfterContextsInitialized()
        {
            base.OnAfterContextsInitialized();
            DataContext.SetData(transform); //Set our own transform.
            _positioning.InstallFor(DataContext);
            if (_runningState is IInitializedSubState state)
            {
                state.Initialize();
            }
        }

        protected override void OnBeginLogic()
        {
            base.OnBeginLogic();
            _runningState.CheckoutEnter(this);
        }

        protected override void OnStopLogic()
        {
            base.OnStopLogic();
            _runningState.CheckoutExit();
        }

        public event Action<ITagOwner, string> onTagAdded
        {
            add => _tagOwner.onTagAdded += value;
            remove => _tagOwner.onTagAdded -= value;
        }

        public event Action<ITagOwner, string> onTagRemoved
        {
            add => _tagOwner.onTagRemoved += value;
            remove => _tagOwner.onTagRemoved -= value;
        }

        public event Action<ITagOwner, string, bool> onTagChanged
        {
            add => _tagOwner.onTagChanged += value;
            remove => _tagOwner.onTagChanged -= value;
        }

        public bool ContainsTag(string t)
        {
            return _tagOwner.ContainsTag(t);
        }

        public void AddTag(string t)
        {
            _tagOwner.AddTag(t);
        }

        public void RemoveTag(string t)
        {
            _tagOwner.RemoveTag(t);
        }
    }
}