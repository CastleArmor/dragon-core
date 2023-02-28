using System.Collections.Generic;
using UnityEngine;

public abstract class SwitchMonoActorState<TSwitch> : InitializedMonoActorState
{
    [SerializeField] private List<DictionarySetupPair<TSwitch, StateField>> _setupPairs;
    private Dictionary<TSwitch, StateField> _states = new Dictionary<TSwitch, StateField>();

    protected StateField GetStateField(TSwitch key)
    {
        return _states[key];
    }

    private IActorState _currentState;
    protected abstract TSwitch SwitchValue {get;}

    protected override void OnInitialize()
    {
        foreach (DictionarySetupPair<TSwitch, StateField> kvp in _setupPairs)
        {
            StateField field = kvp.Value;
            field.InitializeIfNeedsInitialize();
            _states.Add(kvp.Key,field);
        }
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        if (_states.ContainsKey(SwitchValue))
        {
            _currentState = _states[SwitchValue].State;
            if (_currentState != null)
            {
                _currentState.CheckoutEnter(Actor);
            }
        }

        OnAfterInitialStateOperation();
    }

    protected virtual void OnAfterInitialStateOperation()
    {
            
    }

    protected void Evaluate()
    {
        if (_states.ContainsKey(SwitchValue))
        {
            if (_currentState != _states[SwitchValue].State)
            {
                if (_currentState != null)
                {
                    _currentState.CheckoutExit();
                }

                _currentState = _states[SwitchValue].State;
                if (_currentState != null)
                {
                    _currentState.CheckoutEnter(Actor);
                }
            }
        }
        else
        {
            if (_currentState != null)
            {
                _currentState.CheckoutExit();
                _currentState = null;
            }
        }
    }

    protected override void OnExit()
    {
        base.OnExit();

        if (_currentState != null)
        {
            _currentState.CheckoutExit();
            _currentState = null;
        }
    }
}