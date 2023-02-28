using Sirenix.OdinInspector;
using UnityEngine;

public class ActorService : MonoBehaviour
{
    [SerializeField]
    private bool _explicitMain;

    [SerializeField][ShowIf("_explicitMain")] private Actor _explicitReference;
    private IActor _actor;
    public IActor Actor => _actor;
    private bool _unregisteredOrStopped;

    private void OnEnable()
    {
        if (_actor != null) return;
        IActor main = _explicitMain?_explicitReference:GetComponentInParent<IActor>();
        if (!main.IsInitialized)
        {
            main.onInitialize += OnMainDataReady;
        }
        else
        {
            RegisterActor(main);
        }
    }

    private void OnDestroy()
    {
        UnregisterActor();
    }

    private void OnMainDataReady(IActor obj)
    {
        obj.onInitialize -= OnMainDataReady;
        RegisterActor(obj);
    }

    private void RegisterActor(IActor obj)
    {
        _actor = obj;
        _actor.onBegin += BeginActorBehaviour;
        if (_actor.IsRunning)
        {
            BeginActorBehaviour(obj);
        }
        _actor.onStop += StopActorBehaviour;
        _actor.onDestroyActor += OnDestroyActor;
        OnRegisterActor();
    }

    private void UnregisterActor()
    {
        if (Actor != null)
        {
            OnUnregisterMain();
            if (!_unregisteredOrStopped)
            {
                OnUnregisterOrStopAfterBegin();
                _unregisteredOrStopped = true;
            }
            _actor.onBegin -= BeginActorBehaviour;
            _actor.onStop -= StopActorBehaviour;
            _actor.onDestroyActor -= OnDestroyActor;
            _actor = null;
        }
    }

    private void OnDestroyActor(IActor obj)
    {
        UnregisterActor();
    }

    private void StopActorBehaviour(IActor obj)
    {
        OnStopBehaviour();
        if (!_unregisteredOrStopped)
        {
            OnUnregisterOrStopAfterBegin();
            _unregisteredOrStopped = true;
        }
    }

    private void BeginActorBehaviour(IActor obj)
    {
        _unregisteredOrStopped = false;
        OnBeginBehaviour();
    }

    protected virtual void OnBeginBehaviour()
    {
        
    }

    protected virtual void OnStopBehaviour()
    {
        
    }
    
    protected virtual void OnRegisterActor()
    {
        
    }

    protected virtual void OnUnregisterMain()
    {
        
    }

    protected virtual void OnUnregisterOrStopAfterBegin()
    {
        
    }
}