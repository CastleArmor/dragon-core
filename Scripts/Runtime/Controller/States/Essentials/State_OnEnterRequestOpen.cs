using UnityEngine;


public class State_OnEnterRequestOpen : MonoActorState
{
    [SerializeField] private EventField<ToggleableRequestArgs> _openRequest;
    [SerializeField] private string _panelID = "PanelGameplay";
    [SerializeField]
    private OtherToggleableGroupHandling _handling = OtherToggleableGroupHandling.WaitForOtherDisappear;

    protected override void OnGetData()
    {
        base.OnGetData();
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _openRequest.Raise(EventContext,new ToggleableRequestArgs()
        {
            MainID = _panelID,
            OtherHandling = _handling
        });
    }

    protected override void OnExit()
    {
        base.OnExit();
    }
}