using Sirenix.OdinInspector;
using UnityEngine;

public class State_Redirect : InitializedMonoActorState
{
    [GUIColor("GetFieldColor")]
    [SerializeField] private MonoActorState _monoState;
    private bool _isNull;
    private bool _isPreparedNull;

    private Color GetFieldColor()
    {
        if(_monoState == null) return new Color(0,0.7f,1f);
        else return new Color(0,1f,0.7f);
    }

    public bool IsNull
    {
        get
        {
            if (!_isPreparedNull)
            {
                _isNull = _monoState == null;
                _isPreparedNull = true;
            }

            return _isNull;
        }
    }
        
        

    protected override void OnEnter()
    {
        base.OnEnter();
        if (IsNull)
        {
            FinishIfNot();
            return;
        }
        _monoState.onStateFinish += OnFinished;
        _monoState.CheckoutEnter(Actor);
    }

    private void OnFinished(IActorState state)
    {
        FinishIfNot();
    }

    protected override void OnExit()
    {
        base.OnExit();
        if (IsNull) return;
        _monoState.CheckoutExit();
        _monoState.onStateFinish -= OnFinished;
    }
    protected override void OnInitialize()
    {
        if (_monoState is IInitializedSubState initialized)
        {
            if (!initialized.IsInitialized)
            {
                initialized.Initialize();
            }
        }
    }
}