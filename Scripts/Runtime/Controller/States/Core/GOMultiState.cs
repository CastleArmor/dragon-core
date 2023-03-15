using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Dragon.Core
{
    public class GOMultiState : InitializedMonoActorState
    {
        [ShowInInspector][ReadOnly][FoldoutGroup("Info")]
        [HideIf("_showMinimal")]
        private List<IActorState> _instanceStates;
    
        [ShowInInspector][ReadOnly][FoldoutGroup("Info")]
        [HideIf("_showMinimal")]
        private int _finishCount;

        protected override void OnInitialize()
        {
            _instanceStates = new List<IActorState>();
            foreach (IActorState state in transform.GetComponents<IActorState>())
            {
                if (state is GOMultiState multi)
                {
                    if (multi == this) continue;
                }
                _instanceStates.Add(state);
            }

            foreach (IActorState state in _instanceStates)
            {
                if (state is IInitializedSubState initialized)
                {
                    if (!initialized.IsInitialized)
                    {
                        initialized.Initialize();
                    }
                }
            }
        }
    
        private void OnStateFinish(IActorState state)
        {
            _finishCount += 1;
            if (_finishCount == _instanceStates.Count)
            {
                FinishIfNot();
            }
        }
    
        protected override void OnEnter()
        {
            base.OnEnter();
            _finishCount = 0;
            foreach (IActorState state in _instanceStates)
            {
                IActorState instance = state;
        
                instance.CheckoutEnter(Actor);
                instance.onStateFinish += OnStateFinish;
                if (instance.IsFinished)
                {
                    _finishCount += 1;
                }
            }

            if (_finishCount > 0)
            {
                if (_finishCount == _instanceStates.Count)
                {
                    FinishIfNot();
                }
            }

            if(_instanceStates.Count == 0) FinishIfNot();
        }

        protected override void OnExit()
        {
            base.OnExit();
            foreach (IActorState instanceState in _instanceStates)
            {
                instanceState.CheckoutExit();
                instanceState.onStateFinish -= OnStateFinish;
            }
        }
    }
}