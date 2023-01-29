using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public enum ToggleableStatus
{
    Closed,
    Closing,
    Opened,
    Opening
}

[System.Serializable]
public struct ToggleArgs
{
    public IActor Sender;
    public ToggleableDataSet DataSet;
}

public enum ToggleableProcessMode
{
    Immediate,
    Animation,
    Custom
}

[System.Serializable]
public class ToggleableDataSet : InstalledData
{
    [SerializeField] private bool _showEvents;
    [SerializeField] private ToggleableStatus _toggleableStatus = ToggleableStatus.Closed;
    [SerializeField] private ToggleableProcessMode _openingMode = ToggleableProcessMode.Immediate;

    public ToggleableProcessMode OpeningMode
    {
        get => _openingMode;
        set => _openingMode = value;
    }
    [SerializeField] private ToggleableProcessMode _closingMode = ToggleableProcessMode.Immediate;

    public ToggleableProcessMode ClosingMode
    {
        get => _closingMode;
        set => _closingMode = value;
    }
    private bool _handlesCanvasGroup;
    
    [SerializeField][GUIColor(0f,0.7f,1f)] private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get => _canvasGroup;
        set => _canvasGroup = value;
    }
    [SerializeField][GUIColor(0f,0.7f,1f)] private GameObject _toggleObject;

    [SerializeField] private bool _useTransientAnimancer;
    [SerializeField][ShowIf("_useTransientAnimancer")] private AnimancerComponent _transientAnimancer;
    public AnimancerComponent UsedAnimancer => _transientAnimancer;
    
    [SerializeField][ShowIf("IsAnimatedOpening")] private AnimationClip _openingClip;
    [SerializeField][ShowIf("IsAnimatedClosing")] private AnimationClip _closingClip;
    
    [SerializeField] private float _openingSpeed = 1f;
    [SerializeField] private float _closingSpeed = 1f;
    
    [SerializeField][GUIColor(0f,0.7f,1f)] private AnimationClip _openedClip;
    [SerializeField][GUIColor(0f,0.7f,1f)] private AnimationClip _closedClip;
    
    public event Action<ToggleableDataSet> onClosed;
    public event Action<ToggleableDataSet> onBeginClosing;
    public event Action<ToggleableDataSet> onOpened;
    public event Action<ToggleableDataSet> onBeginOpening;
    public event Action<ToggleableDataSet> whileClosing;
    public event Action<ToggleableDataSet> whileOpening;
    public event Action<ToggleableDataSet> onStatusChanged;
    
    [SerializeField][ShowIf("_showEvents")] private EventField _openIfNot;
    [SerializeField][ShowIf("_showEvents")] private EventField _closeIfNot;
    
    public bool UpdatesCanvasGroup => _handlesCanvasGroup;
    
    public bool IsImmediateClosing => _closingMode == ToggleableProcessMode.Immediate;
    public bool IsImmediateOpening => _openingMode == ToggleableProcessMode.Immediate;

    public bool IsAnimatedOpening => _openingMode == ToggleableProcessMode.Animation;
    public bool IsAnimatedClosing => _closingMode == ToggleableProcessMode.Animation;
    
    [SerializeField][ReadOnly] private float _visibility;
    
    public bool IsAnimated => IsAnimatedOpening || IsAnimatedClosing || _openedClip != null || _closedClip != null;
    
    public float Visibility
    {
        get => _visibility;
        set => _visibility = value;
    }

    public bool IsOpeningOrOpened =>
        _toggleableStatus == ToggleableStatus.Opened || _toggleableStatus == ToggleableStatus.Opening;
    
    public bool IsClosingOrClosed => _toggleableStatus == ToggleableStatus.Closed || _toggleableStatus == ToggleableStatus.Closing;
    
    public bool IsVisible => _toggleableStatus != ToggleableStatus.Closed;
    public bool IsOpened => _toggleableStatus == ToggleableStatus.Opened;
    public bool IsOpening => _toggleableStatus == ToggleableStatus.Opening;
    public bool IsClosing => _toggleableStatus == ToggleableStatus.Closing;
    
    public bool IsTransitioning =>
        _toggleableStatus == ToggleableStatus.Closing || _toggleableStatus == ToggleableStatus.Opening;

    public ToggleableStatus Status => _toggleableStatus;

    private AnimancerState _closingState;
    private AnimancerState _openingState;

    private UpdateListener _openingUpdate = new UpdateListener();
    private UpdateListener _closingUpdate = new UpdateListener();

    [Button]
    public void TestOpen()
    {
        OpenIfNot(new EventArgs());
    }
    
    public void OpenIfNot(EventArgs senderArgs)
    {
        if (Context == null) return;
        if (!Context.GetData<IActor>().IsRunning) return;
        if (IsOpeningOrOpened) return;

        _toggleableStatus = ToggleableStatus.Opening;
        onBeginOpening?.Invoke(this);
        onStatusChanged?.Invoke(this);
        
        if (IsImmediateOpening)
        {
            CheckoutOpeningFinished();
        }
        else if (IsAnimatedOpening)
        {
            UsedAnimancer.Animator.enabled = true;
            UsedAnimancer.enabled = true;
            if (_closingUpdate != null)
            {
                _closingUpdate.UnregisterTry();
            }
            _openingState = UsedAnimancer.Play(_openingClip,0f,FadeMode.NormalizedDuration);
            _openingState.Speed = _openingSpeed;
            AnimancerEvent.Sequence seq = new AnimancerEvent.Sequence(_openingState.Events)
            {
                OnEnd = OnOpeningAnimationEnded
            };
            _openingState.Events = seq;
            _openingUpdate.onInvoked = OnAnimationOpeningUpdate;
            _openingUpdate.RegisterTry();
        }
    }

    private void OnAnimationOpeningUpdate()
    {
        UpdateVisibility(_openingState.NormalizedTime);
    }

    private void OnOpeningAnimationEnded()
    {
        _openingUpdate.UnregisterTry();
        CheckoutOpeningFinished();
    }

    private void CheckoutOpeningFinished()
    {
        if (_toggleableStatus == ToggleableStatus.Opened) return;
        _toggleableStatus = ToggleableStatus.Opened;
        ExecuteOpenedView();
        
        onOpened?.Invoke(this);
        onStatusChanged?.Invoke(this);
    }

    private void ExecuteOpenedView()
    {
        Visibility = 1f;

        if (_toggleObject)
        {
            _toggleObject.SetActive(true);
        }
        
        if (UpdatesCanvasGroup)
        {
            _canvasGroup.alpha = Visibility;
            _canvasGroup.blocksRaycasts = true;
            if (_canvasGroup.TryGetComponent(out Canvas canvas))
            {
                canvas.enabled = true;
            }
        }

        if (_openedClip)
        {
            UsedAnimancer.Animator.enabled = true;
            UsedAnimancer.enabled = true;
            UsedAnimancer.Play(_openedClip,0.25f,FadeMode.NormalizedDuration);
        }
        else
        {
            UsedAnimancer.Animator.enabled = false;
            UsedAnimancer.enabled = false;
        }
    }

    /// <summary>
    /// Sent by the state that handles opening or closing.
    /// </summary>
    public void UpdateVisibility(float visibility)
    {
        if (!IsOpening && !IsClosing) return;
        Visibility = Mathf.Clamp01(visibility);
        
        if (UpdatesCanvasGroup)
        {
            _canvasGroup.alpha = Visibility;
        }

        if (IsOpening)
        {
            whileOpening?.Invoke(this);
            if (Visibility >= 1)
            {
                CheckoutOpeningFinished();
            }
        }

        if (IsClosing)
        {
            whileClosing?.Invoke(this);
            if (Visibility <= 0)
            {
                CheckoutClosingFinished();
            }
        }
    }

    [Button]
    public void TestClose()
    {
        CloseIfNot(new EventArgs());
    }
    
    public void CloseIfNot(EventArgs senderArgs)
    {
        if (Context == null) return;
        if (!Context.GetData<IActor>().IsRunning) return;
        if (IsClosingOrClosed) return;
        
        if (UpdatesCanvasGroup)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        if (_openingUpdate!=null)
        {
            _openingUpdate.UnregisterTry();
        }

        _toggleableStatus = ToggleableStatus.Closing;
        onBeginClosing?.Invoke(this);
        onStatusChanged?.Invoke(this);
        
        if (IsImmediateClosing)
        {
            CheckoutClosingFinished();
        }
        else if (IsAnimatedClosing)
        {
            UsedAnimancer.Animator.enabled = true;
            UsedAnimancer.enabled = true;
            _closingState = UsedAnimancer.Play(_closingClip,0,FadeMode.NormalizedDuration);
            _closingState.Speed = _closingSpeed;
            AnimancerEvent.Sequence seq = new AnimancerEvent.Sequence(_closingState.Events)
            {
                OnEnd = OnClosingAnimationEnded
            };
            _closingUpdate.onInvoked = OnAnimationClosingUpdate;
            _closingUpdate.RegisterTry();
            _closingState.Events = seq;
        }
    }

    private void OnAnimationClosingUpdate()
    {
        UpdateVisibility(1-_closingState.NormalizedTime);
    }

    private void OnClosingAnimationEnded()
    {
        _closingUpdate.UnregisterTry();
        CheckoutClosingFinished();
    }

    private void CheckoutClosingFinished()
    {
        if (_toggleableStatus == ToggleableStatus.Closed) return;
        _toggleableStatus = ToggleableStatus.Closed;
        ExecuteClosedView();
        
        onClosed?.Invoke(this);
        onStatusChanged?.Invoke(this);
    }

    private void ExecuteClosedView()
    {
        Visibility = 0f;

        if (_toggleObject)
        {
            _toggleObject.SetActive(false);
        }

        if (UpdatesCanvasGroup)
        {
            _canvasGroup.alpha = Visibility;
            _canvasGroup.blocksRaycasts = false;
            if (_canvasGroup.TryGetComponent(out Canvas canvas))
            {
                canvas.enabled = false;
            }
        }

        if (_closedClip)
        {
            UsedAnimancer.Animator.enabled = true;
            UsedAnimancer.enabled = true;
            UsedAnimancer.Play(_closedClip,0.25f,FadeMode.NormalizedDuration);
        }
        else
        {
            UsedAnimancer.Animator.enabled = false;
            UsedAnimancer.enabled = false;
        }
    }

    protected override void OnInitializeInstanceData()
    {
        base.OnInitializeInstanceData();
        _handlesCanvasGroup = _canvasGroup != null;
        _openIfNot.Register(Context.As<IEventContext>(),OpenIfNot);
        _closeIfNot.Register(Context.As<IEventContext>(),CloseIfNot);

        if (_toggleableStatus == ToggleableStatus.Closing)
        {
            _toggleableStatus = ToggleableStatus.Closed;
        }

        if (_toggleableStatus == ToggleableStatus.Opening)
        {
            _toggleableStatus = ToggleableStatus.Opened;
        }
        
        if (_toggleableStatus == ToggleableStatus.Closed)
        {
            ExecuteClosedView();
        }
        else if (_toggleableStatus == ToggleableStatus.Opened)
        {
            ExecuteOpenedView();
        }
    }

    protected override void OnRemove()
    {
        base.OnRemove();
        _openIfNot.Unregister(Context.As<IEventContext>(),OpenIfNot);
        _closeIfNot.Unregister(Context.As<IEventContext>(),CloseIfNot);
        _openingUpdate.UnregisterTry();
        _closingUpdate.UnregisterTry();
    }
}