using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public class State_OnSlotAvailableRequestRunActor : MonoActorState
    {
        [SerializeField] private DataField<AS_ActorRunner> _actorRunner;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private List<SlotOccupationInfo> _slots;
        [SerializeField] private bool _onExitCancelRunning = true;
        [SerializeField] private DataKey _relationKey;
        private IActor _runningActor;
        private bool _isActorRunning;
        private int _finishFrame;

        protected override void OnGetData()
        {
            base.OnGetData();
            _actorRunner.Get(Actor.pContext);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            _finishFrame = -1;
            _actorRunner.Data.onUsedActorEnded += OnActorEnded;
            TryRunActor();
        }

        private void OnActorEnded(IActor sender, IActor finished)
        {
            if (_runningActor == finished)
            {
                _finishFrame = Time.frameCount;
                _isActorRunning = false;
                _runningActor = null;
            }
            else
            {
                TryRunActor();
            }
        }

        private void TryRunActor()
        {
            if (_finishFrame == Time.frameCount) return;
            Debug.Log("Try run " + _prefab.name);
            if (_isActorRunning)
            {
                return;
            }
            Debug.Log("_isActorRunning passed");
            if (!_actorRunner.Data.CheckSlotsEmpty(_slots))
            {
                return;
            }
            Debug.Log("slot check passed");

            if (Actor.IsBeingDestroyed) return;
            ActorRunResult runResult = _actorRunner.Data.RequestRunning(new ActorRunningArgs()
            {
                DoNotParentToUser = false,
                UsageRequestID = "State_OnSlotAvailableApplyLocomotion",
                OccupationInfos = _slots,
                PrefabOrInstance = _prefab,
                RelationKey = _relationKey
            });
            _runningActor = runResult.RunningInstance;
            _isActorRunning = runResult.IsSuccess;
            Debug.Log("run result = " + _isActorRunning);
        }

        protected override void OnExit()
        {
            base.OnExit();
            if (_onExitCancelRunning)
            {
                if (_isActorRunning)
                {
                    _runningActor.CancelIfNotEnded("State_OnSlotAvailableApplyLocomotion");
                    //This should trigger top side OnActorEnded.
                }
            }
            _actorRunner.Data.onUsedActorEnded -= OnActorEnded;
        }
    }
}