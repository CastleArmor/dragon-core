using Animancer;
using UnityEngine;

public class CoreActor : Actor
{
    [SerializeField] private MonoActorState _runningState;

    protected override void OnInitialize()
    {
        base.OnInitialize();
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
}