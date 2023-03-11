using System.Collections.Generic;
using UnityEngine;

public class State_ActorRunIterator : MonoActorState
{
    [SerializeField] private DataField<AS_ActorRunner> _actorRunner;
    [SerializeField] private DataField<List<GameObject>> _pointList;
    [SerializeField] private List<SlotOccupationInfo> _slots;
    [SerializeField] private DataKey _relationKey;
    [SerializeField] private bool _doNotParentToUser;
    private int _currentIndex;
    private IActor _currentInstance;
    private List<GameObject> PointList => _pointList.Data;
    [SerializeField] private bool _finishActorInsteadOfState;

    protected override void OnGetData()
    {
        base.OnGetData();
        _actorRunner.Get(DataContext);
        _pointList.Get(DataContext);
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _currentIndex = -1;
        MoveNext();
    }

    private void MoveNext()
    {
        _currentIndex += 1;
        if (_currentIndex >= PointList.Count)
        {
            //Finished;
            if (_finishActorInsteadOfState)
            {
                Actor.FinishIfNotEnded("finish");
            }
            else
            {
                FinishIfNot();
            }
            return;
        }

        if (Actor.IsEnded || Actor.IsBeingDestroyed) return;
        ActorRunResult runResult = _actorRunner.Data.RequestRunning(new ActorRunningArgs()
        {
            DoNotMoveToParent = true,
            DoNotParentToUser = _doNotParentToUser,
            UsageRequestID = "State_OnSlotAvailableApplyLocomotion",
            OccupationInfos = _slots,
            PrefabOrInstance = PointList[_currentIndex],
            RelationKey = _relationKey
        });
        _currentInstance = runResult.RunningInstance;
        _currentInstance.onEnded += OnEndedCurrentInstance;
    }

    private void OnEndedCurrentInstance(IActor obj)
    {
        _currentInstance.onEnded -= OnEndedCurrentInstance;
        _currentInstance = null;
        MoveNext();
    }

    protected override void OnExit()
    {
        base.OnExit();
        if (_currentInstance != null)
        {
            _currentInstance.CancelIfNotEnded("Iterator");
        }
    }
}