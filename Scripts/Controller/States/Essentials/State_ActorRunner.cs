using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class State_ActorRunner : MonoActorState
{
    [InfoBox("RequestValidate won't make something run, its there if you want to know before hand.")]
    [SerializeField] private ReturnEventField<ActorRunningArgs, bool> _requestValidateEvent;
    [InfoBox("RequestRun, After validate it will run.")]
    [SerializeField] private ReturnEventField<ActorRunningArgs, ActorRunResult> _requestRunEvent;
    [InfoBox("Runs when both, requestValidate and requestRun")]
    [SerializeField] private EventField<ActorUsageValidateArgs> _validateActorRunning;
    [SerializeField] private DataField<Dictionary<string, List<IActor>>> _runningDictionary;
    [SerializeField] private DataField<UniqueList<IActor>> _runningList;
    private DelegatedObject<bool> _validationObject = new DelegatedObject<bool>();

    [Space]
    [SerializeField] private int _actorListPoolCount = 1;
    [SerializeField] private int _stringListPoolCount = 1;

    private InstancePool<List<IActor>> _actorListPool = new InstancePool<List<IActor>>();
    private InstancePool<List<string>> _stringListPool = new InstancePool<List<string>>();

    private Dictionary<string, List<IActor>> _occupationDictionary = new Dictionary<string, List<IActor>>();
    private Dictionary<IActor, List<string>> _occupierToOccupationList = new Dictionary<IActor, List<string>>();

    protected override void OnEnter()
    {
        base.OnEnter();
        _actorListPool.Create(_actorListPoolCount);
        _stringListPool.Create(_stringListPoolCount);
        _requestRunEvent.Register(EventContext,OnRunCommand);
        _requestValidateEvent.Register(EventContext,OnRequestValidateRun);
    }

    private bool OnRequestValidateRun(ActorRunningArgs arg)
    {
        _validationObject.DelegateObject = true;
        _validateActorRunning.TryRaise(EventContext,new ActorUsageValidateArgs()
        {
            DelegateObject = _validationObject,
            PrefabOrInstance = arg.PrefabOrInstance.GetComponent<IGOInstance>(),
            UsageRequestID = arg.UsageRequestID
        });
        return _validationObject.DelegateObject;
    }

    protected override void OnExit()
    {
        base.OnExit();
        
        _requestRunEvent.Unregister(EventContext,OnRunCommand);
        _requestValidateEvent.Unregister(EventContext,OnRequestValidateRun);

        ActorUsageStandards.StopAllChildActors("Exit",_runningList.Data);
        
        _actorListPool.Clear();
        _stringListPool.Clear();
    }

    private ActorRunResult OnRunCommand(ActorRunningArgs runningArgs)
    {
        _validationObject.DelegateObject = true;
        return ActorUsageStandards.TryStartChildActor(
            Actor,
            runningArgs.PrefabOrInstance.GetComponent<IGOInstance>(),
            runningArgs.UsageRequestID, 
            _validationObject, OnEvaluateCanStart, OnStartConfirmed,
            _runningDictionary.Data, _runningList.Data, 
            OnActorFinishEnded, OnActorCancelEnded, OnBeforeStart,
            runningArgs.DoNotParentToUser,runningArgs.OccupationInfos,
            _occupationDictionary,_occupierToOccupationList,_actorListPool,_stringListPool);
    }

    protected virtual void OnBeforeStart(IActor obj)
    {
        
    }

    protected virtual void OnActorCancelEnded(IActor endedActor)
    {
        ActorUsageStandards.StandardChildActorRelease(
            endedActor,_runningDictionary.Data,
            _runningList.Data,OnActorFinishEnded,OnActorCancelEnded,
            _occupationDictionary,_occupierToOccupationList,_actorListPool,_stringListPool);
    }

    protected virtual void OnActorFinishEnded(IActor endedActor)
    {
        ActorUsageStandards.StandardChildActorRelease(
            endedActor,_runningDictionary.Data,
            _runningList.Data,OnActorFinishEnded,OnActorCancelEnded,
            _occupationDictionary,_occupierToOccupationList,_actorListPool,_stringListPool);
    }

    protected virtual void OnEvaluateCanStart(ActorUsageValidateArgs obj)
    {
        _validateActorRunning.TryRaise(EventContext,obj);
    }

    protected virtual void OnStartConfirmed(ActorUsageEventArgs obj)
    {
        
    }
}