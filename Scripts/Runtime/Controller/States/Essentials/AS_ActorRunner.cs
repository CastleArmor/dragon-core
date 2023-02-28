using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AS_ActorRunner : ActorService
{
    [InfoBox("RequestRun, After validate it will run.")]
    [SerializeField] private ReturnEventField<ActorRunningArgs, ActorRunResult> _requestRunEvent;
    
    /// <summary>
    /// Arg1 = Sender, Arg2 = ActorUsageValidateArgs
    /// </summary>
    public event Action<IActor,ActorUsageValidateArgs> validateActorRunning;
    /// <summary>
    /// Arg1 = Sender, Arg2 = Finished
    /// </summary>
    public event Action<IActor,IActor> onUsedActorEnded;
    
    [SerializeField] private DataField<Dictionary<string, List<IActor>>> _runningDictionary;
    [SerializeField] private DataField<UniqueList<IActor>> _runningList;
    private DelegatedObject<bool> _validationObject = new DelegatedObject<bool>();

    [Space]
    [SerializeField] private int _actorListPoolCount = 1;
    [SerializeField] private int _stringListPoolCount = 1;

    private InstancePool<List<IActor>> _actorListPool = new InstancePool<List<IActor>>();
    private InstancePool<List<string>> _stringListPool = new InstancePool<List<string>>();

    [ShowInInspector][ReadOnly]
    private Dictionary<string, List<IActor>> _occupationDictionary = new Dictionary<string, List<IActor>>();
    [ShowInInspector][ReadOnly]
    private Dictionary<IActor, List<string>> _occupierToOccupationList = new Dictionary<IActor, List<string>>();

    protected override void OnRegisterActor()
    {
        base.OnRegisterActor();
        DataRegistry<AS_ActorRunner>.SetData(Actor.DataContext,this);
        
        _runningList.Get(Actor.DataContext);
        _runningDictionary.Get(Actor.DataContext);
    }

    protected override void OnBeginBehaviour()
    {
        base.OnBeginBehaviour();
        _actorListPool.Create(_actorListPoolCount);
        _stringListPool.Create(_stringListPoolCount);
    }

    protected override void OnUnregisterOrStopAfterBegin()
    {
        base.OnUnregisterOrStopAfterBegin();
        //ActorUsageStandards.StopAllChildActors("Exit",_runningList.Data);
        
        _actorListPool.Clear();
        _stringListPool.Clear();
    }

    [Button]
    public bool ValidateRunning(ActorRunningArgs arg)
    {
        _validationObject.DelegateObject = true;
        validateActorRunning?.Invoke(Actor,new ActorUsageValidateArgs()
        {
            DelegateObject = _validationObject,
            PrefabOrInstance = arg.PrefabOrInstance.GetComponent<IGOInstance>(),
            UsageRequestID = arg.UsageRequestID
        });
        return _validationObject.DelegateObject;
    }

    [Button]
    public ActorRunResult RequestRunning(ActorRunningArgs runningArgs)
    {
        if (Actor.IsBeingDestroyed) return new ActorRunResult();
        _validationObject.DelegateObject = true;
        string relationID = runningArgs.RelationKey ? runningArgs.RelationKey.ID : null;
        return ActorUsageStandards.TryStartChildActor(
            Actor,
            runningArgs.PrefabOrInstance.GetComponent<IGOInstance>(),
            runningArgs.UsageRequestID, 
            _validationObject, OnEvaluateCanStart, OnStartConfirmed,
            _runningDictionary.Data, _runningList.Data, 
            OnActorFinishEnded, OnActorCancelEnded, OnBeforeStart,
            runningArgs.DoNotParentToUser,runningArgs.OccupationInfos,
            _occupationDictionary,_occupierToOccupationList,_actorListPool,_stringListPool,relationID,runningArgs.DoNotMoveToParent);
    }

    public bool CheckSlotsEmpty(List<SlotOccupationInfo> occupationInfos)
    {
        foreach (SlotOccupationInfo info in occupationInfos)
        {
            if (info.Mode == SlotOccupationMode.Additive) continue;
            if (_occupationDictionary.ContainsKey(info.Slot.ID))
            {
                if (_occupationDictionary[info.Slot.ID].Count > 0)
                {
                    return false;
                }
            }       
        }

        return true;
    }

    protected virtual void OnBeforeStart(IActor obj)
    {
        
    }

    protected virtual void OnActorCancelEnded(IActor endedActor)
    {
        if (endedActor == null) return;
        Debug.Log("Cancelled Ended " + endedActor.name);
        ActorUsageStandards.StandardChildActorRelease(
            endedActor,_runningDictionary.Data,
            _runningList.Data,OnActorFinishEnded,OnActorCancelEnded,
            _occupationDictionary,_occupierToOccupationList,_actorListPool,_stringListPool);
        onUsedActorEnded?.Invoke(Actor,endedActor);
    }

    protected virtual void OnActorFinishEnded(IActor endedActor)
    {
        Debug.Log("Finish Ended " + endedActor.name);
        ActorUsageStandards.StandardChildActorRelease(
            endedActor,_runningDictionary.Data,
            _runningList.Data,OnActorFinishEnded,OnActorCancelEnded,
            _occupationDictionary,_occupierToOccupationList,_actorListPool,_stringListPool);
        onUsedActorEnded?.Invoke(Actor,endedActor);
    }

    protected virtual void OnEvaluateCanStart(ActorUsageValidateArgs obj)
    {
        validateActorRunning?.Invoke(Actor,obj);
    }

    protected virtual void OnStartConfirmed(ActorUsageEventArgs obj)
    {
        
    }
}