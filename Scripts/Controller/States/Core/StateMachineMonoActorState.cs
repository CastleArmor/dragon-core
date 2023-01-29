using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITransition
{
    IActorState FromState { get; set; }
    IActorState ToState { get; set; }
    bool GetCondition();
}

public abstract class StateMachineMonoActorState : InitializedMonoActorState
{
    [SerializeField] private StateMachine _stateMachine;
    protected abstract IActorState InitialState { get; }
    private List<ITransition> _transitions = new List<ITransition>();

    protected void Evaluate()
    {
        _stateMachine.Evaluate();
    }

    protected abstract void OnGetTransitions();

    protected void AddDirectTransition(IActorState from, IActorState to, Func<bool> condition)
    {
        _transitions.Add(new SMTransition()
        {
            From = from,
            To = to,
            Condition = condition
        });
    }
    
    protected void AddDirectTransition(IActorState from, IActorState to, Func<bool> condition,Action onTrigger)
    {
        _transitions.Add(new SMTriggerTransition()
        {
            From = from,
            To = to,
            Condition = condition,
            OnTrigger = onTrigger
        });
    }

    protected void AddAnyTransition(IActorState to, Func<bool> condition)
    {
        _transitions.Add(new SMTransition()
        {
            From = null,
            To = to,
            Condition = condition
        });
    }
    
    protected void AddAnyTransition(IActorState to, Func<bool> condition, Action onTrigger)
    {
        _transitions.Add(new SMTriggerTransition()
        {
            From = null,
            To = to,
            Condition = condition,
            OnTrigger = onTrigger
        });
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _stateMachine.Enter(Actor,InitialState);
    }

    protected override void OnExit()
    {
        base.OnExit();
        _stateMachine.Exit();
    }

    protected override void OnInitialize()
    {
        OnGetTransitions();
        _stateMachine.InstallTransitions(_transitions);
    }
}