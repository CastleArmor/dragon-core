using Sirenix.OdinInspector;
using UnityEngine;

public class ToggleableActor : Actor
{
    [SerializeField][InlineProperty][HideLabel] private ToggleableDataSet _toggleableData;
    [SerializeField] [FoldoutGroup("States")] private StateField _running;
    [SerializeField] [FoldoutGroup("States")] private StateField _closed;
    [SerializeField] [FoldoutGroup("States")] private StateField _closing;
    [SerializeField] [FoldoutGroup("States")] private StateField _opened;
    [SerializeField] [FoldoutGroup("States")] private StateField _opening; 
    [SerializeField] [FoldoutGroup("States")] private StateField _visible;
    [SerializeField] [FoldoutGroup("States")] private StateField _closingAndClosed;
    [SerializeField] [FoldoutGroup("States")] private StateField _openingAndOpened;
    public bool IsAnimated => _toggleableData.IsAnimated;
    
    private IActorState _currentState;

    private void OnValidate()
    {
        if(_toggleableData != null)
        {
            if (_toggleableData.CanvasGroup == null)
            {
                _toggleableData.CanvasGroup = GetComponent<CanvasGroup>();
            }
        }
    }

    private void SwitchStateIfNotEntered(IActorState toState)
    {
        if (_currentState == toState) return;
        if (_currentState != null)
        {
            _currentState.CheckoutExit();
        }
        _currentState = toState;
        if (_currentState != null)
        {
            _currentState.CheckoutEnter(this);
        }
    }
    
    protected override void OnInitialize()
    {
        if (_toggleableData.CanvasGroup == null)
        {
            _toggleableData.CanvasGroup = GetComponent<CanvasGroup>();
        }
        DataContext.SetData(_toggleableData);
        base.OnInitialize();
        _running.InitializeIfNeedsInitialize();
        _closed.InitializeIfNeedsInitialize();
        _closing.InitializeIfNeedsInitialize();
        _opened.InitializeIfNeedsInitialize();
        _opening.InitializeIfNeedsInitialize();
        _visible.InitializeIfNeedsInitialize();
        _closingAndClosed.InitializeIfNeedsInitialize();
        _openingAndOpened.InitializeIfNeedsInitialize();
    }

    protected override void OnBeginLogic()
    {
        base.OnBeginLogic();
        _toggleableData.onStatusChanged+=OnStatusChanged;
        _currentState = InitialState;
        _running.TryCheckoutEnter(this);
        if (_currentState != null)
        {
            _currentState.CheckoutEnter(this);
        }
    }

    protected override void OnStopLogic()
    {
        base.OnStopLogic();
        _toggleableData.onStatusChanged-=OnStatusChanged;
        _running.TryCheckoutExit(this);
        if (_currentState != null)
        {
            _currentState.CheckoutExit();
            _currentState = null;
        }
    }

    private void OnStatusChanged(ToggleableDataSet dataSet)
    {
        switch (_toggleableData.Status)
        {
            case ToggleableStatus.Closed : 
                SwitchStateIfNotEntered(_closed.State);
                break;
            case ToggleableStatus.Closing : 
                SwitchStateIfNotEntered(_closing.State);
                break;
            case ToggleableStatus.Opened : 
                SwitchStateIfNotEntered(_opened.State);
                break;
            case ToggleableStatus.Opening : 
                SwitchStateIfNotEntered(_opening.State);
                break;
            default: return;
        }
        _visible.CheckoutIf(this,_toggleableData.IsVisible);
        _closingAndClosed.CheckoutIf(this,_toggleableData.IsClosingOrClosed);
        _openingAndOpened.CheckoutIf(this,_toggleableData.IsOpeningOrOpened);
    }

    protected IActorState InitialState
    {
        get
        {
            switch (_toggleableData.Status)
            {
                case ToggleableStatus.Closed : return _closed.State;
                case ToggleableStatus.Closing : return _closing.State;
                case ToggleableStatus.Opened : return _opened.State;
                case ToggleableStatus.Opening : return _opening.State;
                default: return _closed.State;
            }
        }
    }
}