using UnityEngine;

public class State_SimpleLogicRunningHandler : MonoActorState
{
    [SerializeField] private DataField<D_LogicRunning> _gameMode;

    [SerializeField] private ReturnEventField<string> _requestRestartGameModeEvent;
    [SerializeField] private ReturnEventField<string> _requestStartGameModeEvent;
    [SerializeField] private ReturnEventField<string> _requestFinalizeGameModeEvent;
    [SerializeField] private ReturnEventField<string> _requestEndGameModeEvent;
    [SerializeField] private EventField _onStartGameModeEvent;
    [SerializeField] private EventField _onEndGameModeEvent;
    [SerializeField] private EventField _onFinalizeGameModeEvent;
    
    protected override void OnGetData()
    {
        base.OnGetData();
        _gameMode.Get(DataContext);
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _gameMode.Data.ChangeStatus(LogicStatus.Greet);
        _requestStartGameModeEvent.Register(EventContext,OnRequestStartGameMode);
        _requestEndGameModeEvent.Register(EventContext,OnRequestEndGameMode);
        _requestRestartGameModeEvent.Register(EventContext,OnRequestRestartGameMode);
        _requestFinalizeGameModeEvent.Register(EventContext,OnFinalizeGameMode);
    }

    protected override void OnExit()
    {
        base.OnExit();
        _requestStartGameModeEvent.Unregister(EventContext,OnRequestStartGameMode);
        _requestEndGameModeEvent.Unregister(EventContext,OnRequestEndGameMode);
        _requestRestartGameModeEvent.Unregister(EventContext,OnRequestRestartGameMode);
        _requestFinalizeGameModeEvent.Unregister(EventContext,OnFinalizeGameMode);
    }

    private string OnFinalizeGameMode(EventArgs arg)
    {
        if (_gameMode.Data.Status == LogicStatus.Finalized) return "AlreadyFinalized";
        if (_gameMode.Data.Status != LogicStatus.Ended) return "Error:MustBeInEnded";
        _gameMode.Data.ChangeStatus(LogicStatus.Finalized);
        _onFinalizeGameModeEvent.Raise(EventContext);
        return "Confirmed";
    }

    private string OnRequestRestartGameMode(EventArgs obj)
    {
        switch (_gameMode.Data.Status)
        {
            case LogicStatus.Ended : _requestFinalizeGameModeEvent.Raise(EventContext);
                break;
            case LogicStatus.Finalized :
                //Not really stays in this.
                return "AboutToLoadScene";
                break;
            case LogicStatus.Running : 
                _requestEndGameModeEvent.Raise(EventContext);
                _requestFinalizeGameModeEvent.Raise(EventContext);
                break;
        }
        return "Confirmed";
    }
    
    private string OnRequestEndGameMode(EventArgs obj)
    {
        if (_gameMode.Data.Status == LogicStatus.Ended) return "AlreadyEnded";
        if (_gameMode.Data.Status != LogicStatus.Running) return "Error:NotInRunning";
        _gameMode.Data.ChangeStatus(LogicStatus.Stopped);
        _gameMode.Data.ChangeStatus(LogicStatus.Ended);
        _onEndGameModeEvent.Raise(EventContext);
        return "Confirmed";
    }

    private string OnRequestStartGameMode(EventArgs obj)
    {
        if (_gameMode.Data.Status == LogicStatus.Running) return "AlreadyRunning";
        if (_gameMode.Data.Status != LogicStatus.Greet) return "Error:NotInGreet";
        _gameMode.Data.ChangeStatus(LogicStatus.Running);
        _onStartGameModeEvent.Raise(EventContext);
        return "Confirmed";
    }
}