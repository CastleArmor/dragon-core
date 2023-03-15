using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    public abstract class ExplicitBooleanBranchMonoActorState : BranchMonoActorState
    {
        [SerializeField] private StateField _true;
        [SerializeField] private StateField _false;

        public abstract bool Boolean
        {
            get;
        }

        public abstract event Action onChangeEvent;

        protected override void OnInitialize()
        {
            _true.InitializeIfNeedsInitialize();
            _false.InitializeIfNeedsInitialize();
        }

        protected override IActorState InitialState =>
            Boolean ? _true.State : _false.State;

        protected override void OnEnter()
        {
            base.OnEnter();
            onChangeEvent += Evaluate;
        }

        protected override void OnExit()
        {
            base.OnExit();
            onChangeEvent -= Evaluate;
        }

        private void Evaluate()
        {
            if (Boolean)
            {
                SwitchStateIfNotEntered(_true.State);
            }
            else
            {
                SwitchStateIfNotEntered(_false.State);
            }
        }
    }
    public abstract class BranchMonoActorState : InitializedMonoActorState
    {
        [ShowInInspector][ReadOnly]
        private IActorState _currentState;
        protected abstract IActorState InitialState {get;}

        protected override void OnEnter()
        {
            base.OnEnter();
            _currentState = InitialState;
            if (_currentState != null)
            {
                _currentState.CheckoutEnter(Actor);
            }

            OnAfterInitialStateEntered();
        }

        protected virtual void OnAfterInitialStateEntered()
        {
            
        }

        protected void SwitchStateIfNotEntered(IActorState toState)
        {
            if (_currentState == toState) return;
            if (_currentState != null)
            {
                _currentState.CheckoutExit();
            }
            _currentState = toState;
            if (_currentState != null)
            {
                _currentState.CheckoutEnter(Actor);
            }
        }

        protected override void OnExit()
        {
            base.OnExit();
            _currentState.CheckoutExit();
            _currentState = null;
        }
    }
}