using System.Collections.Generic;
using UnityEngine;

public class State_MaintainActors : MonoActorState
{
    [SerializeField] private List<GameObject> _list;
    protected override void OnEnter()
    {
        base.OnEnter();
        foreach (GameObject go in _list)
        {
            go.GetComponent<IActor>().InitializeIfNot();
            go.GetComponent<IActor>().BeginIfNot();
        }
    }

    protected override void OnExit()
    {
        base.OnExit();
        foreach (GameObject go in _list)
        {
            go.GetComponent<IActor>().InitializeIfNot();
            go.GetComponent<IActor>().StopIfNot();
        }
    }
}