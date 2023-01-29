using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public struct SMTransition : ITransition
{
    public Func<bool> Condition;
    public IActorState From;
    public IActorState To;

    public IActorState FromState
    {
        get => From;
        set => From = value;
    }

    public IActorState ToState
    {
        get => To;
        set => To = value;
    }
    public bool GetCondition()
    {
        return Condition.Invoke();
    }
}

public struct SMTriggerTransition : ITransition
{
    public Func<bool> Condition;
    public Action OnTrigger;
    public IActorState From;
    public IActorState To;

    public IActorState FromState
    {
        get => From;
        set => From = value;
    }

    public IActorState ToState
    {
        get => To;
        set => To = value;
    }
    public bool GetCondition()
    {
        bool isPassing = Condition.Invoke();
        if (isPassing)
        {
            OnTrigger.Invoke();
        }
        return isPassing;
    }
}

[System.Serializable]
public class StateMachine
{
    [ShowInInspector][ReadOnly]
    //Make sure you don't have circularity.
    private bool _recursiveEvaluate = true;
    
    private IActor _actor;
    public IActor Actor => _actor;

    [ShowInInspector][ReadOnly]
    private readonly Dictionary<IActorState, List<ITransition>> _direct = new Dictionary<IActorState, List<ITransition>>();
    
    [ShowInInspector][ReadOnly]
    private readonly List<ITransition> _any = new List<ITransition>();

    [ShowInInspector][ReadOnly]
    private IActorState _currentState;

    public void InstallTransitions(List<ITransition> transitionList)
    {
        for (var i = 0; i < transitionList.Count; i++)
        {
            var transition = transitionList[i];
            if (transition.ToState == null)
            {
                transition.ToState = new EmptySimpleState();
            }
            else
            {
                transition.ToState.TryInitializeIfInitializedSubState();
            }
            if (transition.FromState != null)
            {
                transition.FromState.TryInitializeIfInitializedSubState();
                if (_direct.ContainsKey(transition.FromState))
                {
                    _direct[transition.FromState].Add(transition);
                }
                else
                {
                    _direct.Add(transition.FromState,new List<ITransition>(){transition});
                }
            }
            else
            {
                _any.Add(transition);
            }
        }
    }
    
    public void Enter(IActor actor, IActorState initialState)
    {
        _actor = actor;

        _currentState = initialState;
        _currentState.CheckoutEnter(Actor);
    }

    public void Exit()
    {
        _currentState.CheckoutExit();
    }

    public void Evaluate()
    {
        bool hasSwitchedThisEvaluate = false;
        for (var i = 0; i < _any.Count; i++)
        {
            ITransition transition = _any[i];

            if (transition.ToState == _currentState) continue;
            if (transition.GetCondition())
            {
                SwitchState(transition.ToState);
                hasSwitchedThisEvaluate = true;
                if (_recursiveEvaluate)
                {
                    Evaluate();
                }
            }
        }
        
        if (!hasSwitchedThisEvaluate)
        {
            if (!_direct.ContainsKey(_currentState)) return;
            for (var i = 0; i < _direct[_currentState].Count; i++)
            {
                var transition = _direct[_currentState][i];
                if (transition.GetCondition())
                {
                    SwitchState(transition.ToState);
                    hasSwitchedThisEvaluate = true;
                    if (_recursiveEvaluate)
                    {
                        Evaluate();
                    }
                }
            }
        }
    }

    private void SwitchState(IActorState state)
    {
        if (_currentState != null)
        {
            _currentState.CheckoutExit();
        }

        _currentState = state;
        _currentState.CheckoutEnter(Actor);
    }
}