using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public abstract class State_RunActor : MonoActorState
    {
        [SerializeField] protected DataField<AS_ActorRunner> _actorRunner;
        [SerializeField] protected DataField<GameObject> _prefabOrInstance;
        [SerializeField] protected List<SlotOccupationInfo> _slots;
        [SerializeField] protected DataKey _relationKey;
        [SerializeField] protected bool _doNotParentToUser;
        protected IActor _currentInstance;

        protected override void OnGetData()
        {
            base.OnGetData();
            _actorRunner.Get(pContext);
            _prefabOrInstance.Get(pContext);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
        }

        protected void RunActor()
        {
            ActorRunResult runResult = _actorRunner.Data.RequestRunning(new ActorRunningArgs()
            {
                DoNotMoveToParent = true,
                DoNotParentToUser = _doNotParentToUser,
                UsageRequestID = "State_OnSlotAvailableApplyLocomotion",
                OccupationInfos = _slots,
                PrefabOrInstance = _prefabOrInstance.Data,
                RelationKey = _relationKey
            });
            _currentInstance = runResult.RunningInstance;
            _currentInstance.onEnded += OnEndedCurrentInstance;
        }

        protected virtual void OnEndedCurrentInstance(IActor obj)
        {
            _currentInstance.onEnded -= OnEndedCurrentInstance;
            _currentInstance = null;
        }

        protected override void OnExit()
        {
            base.OnExit();
            if (_currentInstance != null)
            {
                _currentInstance.onEnded -= OnEndedCurrentInstance;
                _currentInstance.CancelIfNotEnded("State_RunActor");
                _currentInstance = null;
            }
        }
    }
}