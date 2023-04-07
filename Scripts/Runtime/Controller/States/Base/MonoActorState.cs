using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    [Flags]
    public enum StateFlags
    {
        Nothing =0,
        Running =1<<0,
        Finished=1<<1,
        Initialized=1<<2
    }

    public abstract class MonoActorState : MonoBehaviour, IActorState
    {
        private bool _showMinimal = true;
    
        [ShowInInspector][HorizontalGroup("Status")][DisplayAsString][PropertyOrder(-1000)][HideLabel]
        public virtual StateFlags Flags
        {
            get
            {
                StateFlags flags = StateFlags.Nothing;
                if (_isRunning)
                {
                    flags |= StateFlags.Running;
                }
                if (_isFinished)
                {
                    flags |= StateFlags.Finished;
                }

                return flags;
            }
        }
    
        private IActor _actor;
        public IActor Actor => _actor;

        private IGOInstancePoolRegistry _goPool;
        public IGOInstancePoolRegistry GOPool => _goPool;
    
        private IContext _context;
        public IContext pContext => _context;

        public T Get<T>(DataField<T> field)
        {
            return field.Get(pContext);
        }

        public void Set<T>(DataField<T> field,T value)
        {
            field.Set(pContext, value);
        }
    
        private bool _isRunning;
        public bool IsRunning => _isRunning;
        private bool _isFinished;
        public bool IsFinished => _isFinished;

        private void InsertActor(IActor actor)
        {
            _actor = actor;
            _goPool = _actor.GOPool;
            _context = _actor.pContext;
        }
    
        public void CheckoutEnter(IActor actor)
        {
            if (_isRunning) return;
            _isRunning = true;
            _isFinished = false;
            InsertActor(actor);
            OnGetData();
            OnEnter();
        }

        protected virtual void OnGetData()
        {
        
        }

        protected virtual void OnEnter()
        {
        
        }

        public void CheckoutExit()
        {
            if (!_isRunning) return;
            OnExit();
            _isRunning = false;
        }

        protected virtual void OnExit()
        {
        
        }

        public void CheckoutInstant(IActor main)
        {
            CheckoutEnter(main);
            CheckoutExit();
        }

        protected void RegisterUpdate(EUpdateType updateType, Action action)
        {
            switch (updateType)
            {
                case EUpdateType.Update : StaticUpdate.onUpdate += action;
                    break;
                case EUpdateType.FixedUpdate : StaticUpdate.onFixedUpdate += action;
                    break;
                case EUpdateType.LateUpdate : StaticUpdate.onLateUpdate += action;
                    break;
            }
        }
    
        protected void UnregisterUpdate(EUpdateType updateType, Action action)
        {
            switch (updateType)
            {
                case EUpdateType.Update : StaticUpdate.onUpdate -= action;
                    break;
                case EUpdateType.FixedUpdate : StaticUpdate.onFixedUpdate -= action;
                    break;
                case EUpdateType.LateUpdate : StaticUpdate.onLateUpdate -= action;
                    break;
            }
        }
    
        protected void FinishIfNot()
        {
            if (_isFinished) return;
            _isFinished = true;
            onStateFinish?.Invoke(this);
        }

        public event Action<IActorState> onStateFinish;
    }
}