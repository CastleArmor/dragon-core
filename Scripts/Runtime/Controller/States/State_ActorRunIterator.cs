using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    
    [SerializeField][ShowIf("ShowFinishActionField")] private FinishAction _finishAction;
    protected virtual bool ShowFinishActionField => true;
    protected virtual FinishAction FinishActionMode => _finishAction;

    public enum FinishAction
    {
        FinishState = 0,
        FinishActor = 1,
        Loop = 2,
        OverrideCustom = 99
    }

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

    /// <summary>
    /// Return true if MoveNext should continue, false if it should return.
    /// </summary>
    /// <returns></returns>
    protected virtual bool OnCustomFinish()
    {
        return false;
    }

    protected virtual void OpIncrementCurrentIndexOnMoveNext()
    {
        _currentIndex += 1;
    }

    private void MoveNext()
    {
        OpIncrementCurrentIndexOnMoveNext();
        if (_currentIndex >= PointList.Count)
        {
            switch (_finishAction)
            {
                case FinishAction.FinishActor: Actor.FinishIfNotEnded("finish");
                    return;
                    break;
                case FinishAction.FinishState: FinishIfNot();
                    return;
                    break;
                case FinishAction.Loop: _currentIndex = 0;
                    break;
                case FinishAction.OverrideCustom:
                    if (!OnCustomFinish())
                    {
                        return;
                    }
                    break;
            }
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
        //For instant finishes.
        if (Actor.IsEnded)
        {
            _currentInstance.onEnded -= OnEndedCurrentInstance;
            _currentInstance.CancelIfNotEnded("Iterator");
            _currentInstance = null;
        }
        else if (_currentInstance.IsEnded)
        {
            if (Actor.IsEnded)
            {
                _currentInstance.onEnded -= OnEndedCurrentInstance;
                _currentInstance.CancelIfNotEnded("Iterator");
                _currentInstance = null;
            }
            else
            {
                OnEndedCurrentInstance(_currentInstance);
            }
        }
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
            _currentInstance.onEnded -= OnEndedCurrentInstance;
            _currentInstance.CancelIfNotEnded("Iterator");
            _currentInstance = null;
        }
    }
}