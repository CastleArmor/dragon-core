using UnityEngine;

namespace Dragon.Core
{
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
            _gameMode.Get(pContext);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            _gameMode.Data.ChangeStatus(LogicStatus.Greet);
            _requestStartGameModeEvent.Register(pContext,OnRequestStartGameMode);
            _requestEndGameModeEvent.Register(pContext,OnRequestEndGameMode);
            _requestRestartGameModeEvent.Register(pContext,OnRequestRestartGameMode);
            _requestFinalizeGameModeEvent.Register(pContext,OnFinalizeGameMode);
        }

        protected override void OnExit()
        {
            base.OnExit();
            _requestStartGameModeEvent.Unregister(pContext,OnRequestStartGameMode);
            _requestEndGameModeEvent.Unregister(pContext,OnRequestEndGameMode);
            _requestRestartGameModeEvent.Unregister(pContext,OnRequestRestartGameMode);
            _requestFinalizeGameModeEvent.Unregister(pContext,OnFinalizeGameMode);
        }

        private string OnFinalizeGameMode(EventArgs arg)
        {
            if (_gameMode.Data.Status == LogicStatus.Finalized) return "AlreadyFinalized";
            if (_gameMode.Data.Status != LogicStatus.Ended) return "Error:MustBeInEnded";
            _gameMode.Data.ChangeStatus(LogicStatus.Finalized);
            _onFinalizeGameModeEvent.Raise(pContext);
            return "Confirmed";
        }

        private string OnRequestRestartGameMode(EventArgs obj)
        {
            switch (_gameMode.Data.Status)
            {
                case LogicStatus.Ended : _requestFinalizeGameModeEvent.Raise(pContext);
                    break;
                case LogicStatus.Finalized :
                    //Not really stays in this.
                    return "AboutToLoadScene";
                    break;
                case LogicStatus.Running : 
                    _requestEndGameModeEvent.Raise(pContext);
                    _requestFinalizeGameModeEvent.Raise(pContext);
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
            _onEndGameModeEvent.Raise(pContext);
            return "Confirmed";
        }

        private string OnRequestStartGameMode(EventArgs obj)
        {
            if (_gameMode.Data.Status == LogicStatus.Running) return "AlreadyRunning";
            if (_gameMode.Data.Status != LogicStatus.Greet) return "Error:NotInGreet";
            _gameMode.Data.ChangeStatus(LogicStatus.Running);
            _onStartGameModeEvent.Raise(pContext);
            return "Confirmed";
        }
    }
}