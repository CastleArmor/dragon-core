using UnityEngine;
//
public class State_LogicRunningSwitch : SwitchMonoActorState<LogicStatus>
{
    [SerializeField] private DataField<D_LogicRunning> _gameModeData;
    protected override LogicStatus SwitchValue => _gameModeData.Data.Status;

    protected override void OnGetData()
    {
        base.OnGetData();
        _gameModeData.Get(DataContext);
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _gameModeData.Data.onStatusChanged += OnStatusChanged;
    }

    protected override void OnExit()
    {
        base.OnExit();
        _gameModeData.Data.onStatusChanged -= OnStatusChanged;
    }

    private void OnStatusChanged(D_LogicRunning arg1, LogicStatus arg2, LogicStatus arg3)
    {
        Evaluate();
    }
}
