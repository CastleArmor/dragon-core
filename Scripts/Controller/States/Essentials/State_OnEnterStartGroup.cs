using System.Collections.Generic;
using UnityEngine;

public class State_OnEnterStartGroup : MonoActorState
{
    [SerializeField] private DataField<List<IActor>> _group;
    protected override void OnEnter()
    {
        base.OnEnter();
        _group.Get(DataContext);
        foreach (IActor actor in _group.Data)
        {
            actor.InitializeIfNot();
            actor.BeginIfNot();
        }
    }
}