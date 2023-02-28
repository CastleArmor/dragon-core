using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ToggleableGroupControlState : MonoActorState
{
    [SerializeField] private bool _showEvents;
    [SerializeField][ShowIf("_showEvents")] private EventField _openPrevious;
    
    [FoldoutGroup("Request Signals")][ShowIf("_showEvents")]
    [SerializeField] private EventField<ToggleableRequestArgs> _openRequest;
    [FoldoutGroup("Request Signals")][ShowIf("_showEvents")]
    [SerializeField] private EventField<ToggleableRequestArgs> _openImmediateRequest;
    [FoldoutGroup("Request Signals")][ShowIf("_showEvents")]
    [SerializeField] private EventField _closeRequest;
    [FoldoutGroup("Request Signals")][ShowIf("_showEvents")]
    [SerializeField] private EventField _closeImmediateRequest;
    [FoldoutGroup("Command Signals")][ShowIf("_showEvents")]
    [SerializeField] private EventField _openCommand;
    [FoldoutGroup("Command Signals")][ShowIf("_showEvents")]
    [SerializeField] private EventField _closeCommand;
    
    [SerializeField]
    private DataField<Dictionary<string,IContext>> _groupDictionary;
    private Dictionary<string,IContext> GroupDictionary => _groupDictionary.Data;
    
    [SerializeField]
    private DataField<ToggleableGroupDataSet> _groupData;
    private ToggleableGroupDataSet GroupData => _groupData.Data;

    [Space]
    [InfoBox("Used to pull data from toggleables in list.")]
    [SerializeField] private DataField<ToggleableDataSet> _toggleableData;

    [Button]
    public void TestRequestOpen(GameObject toggleable)
    {
        _openRequest.Raise(EventContext,new ToggleableRequestArgs()
        {
            MainID = toggleable.GetComponent<IGOInstance>().ObjectTypeID,
            OtherHandling = OtherToggleableGroupHandling.WaitForOtherDisappear
        });
    }

    private void OnOpenPreviousMain(EventArgs senderArgs)
    {
        if (GroupData.Previous != null)
        {
            _openRequest.Raise(EventContext,new ToggleableRequestArgs()
            {
                MainID = GroupData.Previous.As<IUnityComponent>().GetComponent<IGOInstance>().ObjectTypeID,
                OtherHandling = OtherToggleableGroupHandling.WaitForOtherDisappear
            });
        }
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        _groupDictionary.Get(DataContext);
        _groupData.Get(DataContext);

        GroupData.onAddedToggleable += OnAddedToggleable;
        GroupData.onRemoveToggleable += OnRemoveToggleable;
        GroupData.WaitingForImmediateOpening = null;
        GroupData.WaitingForOpening = null;

        bool didFirstMemberOp = false;
        foreach (GameObject toggleable in GroupData.Toggleables)
        {
            IActor main = Actor.StartTransientUsedMain(toggleable,out Transform oldParent);
            GroupDictionary.Add(toggleable.GetComponent<IGOInstance>().ObjectTypeID,toggleable.GetComponent<IActor>().DataContext);

            ToggleableDataSet data = _toggleableData.Get(main.DataContext);
            data.onOpened += OnMainOpened;
            data.onClosed += OnMainClosed;
            data.onBeginClosing += OnClosing;
            data.onBeginOpening += OnOpening;

            if (_groupData.Data.OpenFirstMemberAtEnter)
            {
                if (!didFirstMemberOp)
                {
                    didFirstMemberOp = true;
                    OpenPanel(main.DataContext);
                    GroupData.Opened = main.DataContext;
                }
                else
                {
                    ClosePanelImmediate(main.DataContext);
                }
            }
        }

        //Events
        _openPrevious.Register(Actor.EventContext,OnOpenPreviousMain);
        
        //Requests
        _openRequest.Register(Actor.EventContext,OnRequestOpen);
        _openImmediateRequest.Register(Actor.EventContext,OnRequestOpenImmediate);
        _closeRequest.Register(Actor.EventContext,OnRequestClose);
        _closeImmediateRequest.Register(Actor.EventContext,OnRequestCloseImmediate);
    }

    private void OnRemoveToggleable(GameObject toggleable)
    {
        if (!GroupDictionary.ContainsKey(toggleable.GetComponent<IGOInstance>().ObjectTypeID)) return;
        
        IContext context = GroupDictionary[toggleable.GetComponent<IGOInstance>().ObjectTypeID];

        ToggleableDataSet data = _toggleableData.Get(context.As<IDataContext>());
        data.onOpened -= OnMainOpened;
        data.onClosed -= OnMainClosed;
        data.onBeginClosing -= OnClosing;
        data.onBeginOpening -= OnOpening;

        if (GroupData.Opened == context)
        {
            GroupData.Opened = null;
        }
        if (GroupData.Opening == context)
        {
            GroupData.Opening = null;
        }
        if (GroupData.Closing == context)
        {
            GroupData.Closing = null;
        }
        if (GroupData.Previous == context)
        {
            GroupData.Previous = null;
        }
        if (GroupData.WaitingForOpening == context)
        {
            GroupData.WaitingForOpening = null;
        }
        if (GroupData.WaitingForImmediateOpening == context)
        {
            GroupData.WaitingForImmediateOpening = null;
        }
        
        GroupDictionary.Remove(toggleable.GetComponent<IGOInstance>().ObjectTypeID);
        toggleable.GetComponent<IActor>().CancelIfNotEnded("Cancelled");
    }

    private void OnAddedToggleable(GameObject toggleable)
    {
        IActor context = Actor.StartTransientUsedMain(toggleable,out Transform oldParent);

        ToggleableDataSet data = _toggleableData.Get(context.DataContext);
        data.onOpened += OnMainOpened;
        data.onClosed += OnMainClosed;
        data.onBeginClosing += OnClosing;
        data.onBeginOpening += OnOpening;
    }

    protected override void OnExit()
    {
        base.OnExit();
        
        foreach (GameObject toggleable in GroupData.Toggleables)
        {
            IContext context = GroupDictionary[toggleable.GetComponent<IGOInstance>().ObjectTypeID];

            ToggleableDataSet data = _toggleableData.Get(context.As<IDataContext>());
            data.onOpened -= OnMainOpened;
            data.onClosed -= OnMainClosed;
            data.onBeginClosing -= OnClosing;
            data.onBeginOpening -= OnOpening;
            
            GroupDictionary.Remove(toggleable.GetComponent<IGOInstance>().ObjectTypeID);
            toggleable.GetComponent<IActor>().CancelIfNotEnded("Cancelled");
        }
        
        _openPrevious.Unregister(EventContext,OnOpenPreviousMain);
        
        //Requests
        _openRequest.Unregister(EventContext,OnRequestOpen);
        _openImmediateRequest.Unregister(EventContext,OnRequestOpenImmediate);
        _closeRequest.Unregister(EventContext,OnRequestClose);
        _closeImmediateRequest.Unregister(EventContext,OnRequestCloseImmediate);
    }

    private void OnMainOpened(ToggleableDataSet dataSet)
    {
        if (GroupData.Opening == dataSet.DataContext)
        {
            GroupData.Opening = null;
        }
        
        GroupData.Opened = dataSet.DataContext;
    }
    private void OnMainClosed(ToggleableDataSet dataSet)
    {
        if (GroupData.Closing == dataSet.DataContext)
        {
            
        }
        GroupData.Previous = dataSet.DataContext;
        
        if (GroupData.Closing == GroupData.Opened)
        {
            GroupData.Closing = null;
            GroupData.Opened = null;
            if (GroupData.WaitingForOpening != null)
            {
                OpenPanel(GroupData.WaitingForOpening);
                GroupData.WaitingForOpening = null;
            }
            if (GroupData.WaitingForImmediateOpening != null)
            {
                OpenPanelImmediate(GroupData.WaitingForImmediateOpening);
                GroupData.WaitingForImmediateOpening = null;
            }
        }
    }
    
    private void OnOpening(ToggleableDataSet dataSet)
    {
        GroupData.Opening = dataSet.DataContext;
    }
    
    private void OnClosing(ToggleableDataSet dataSet)
    {
        GroupData.Closing = dataSet.DataContext;
    }

    private void OnRequestOpen(EventArgs senderArgs,ToggleableRequestArgs arg)
    {
        if (!GroupDictionary.ContainsKey(arg.MainID))
        {
            Debug.LogError(Actor.name + " - ToggleableGroupControlState's GroupDictionary doesn't contain " + arg.MainID);
            return;
        }
        IContext context = GroupDictionary[arg.MainID];
        if (GroupData.Opening == context) return; //Already appearing
        if (GroupData.Opened == context) return; //Already opened
        if (GroupData.Closing == context)
        {
            ClosePanelImmediate(GroupData.Closing);
        }
        //TODO: HAVE HERE REQUEST SPAM HANDLING

        if (GroupData.Opened != null)
        {
            if (arg.OtherHandling.HasFlag(OtherToggleableGroupHandling.WaitForOtherDisappear))
            {
                GroupData.WaitingForOpening = context;
            }
            else
            {
                OpenPanel(context);
            }
            if (arg.OtherHandling.HasFlag(OtherToggleableGroupHandling.OtherClosesImmediately))
            {
                ClosePanelImmediate(GroupData.Opened);
            }
            else
            {
                ClosePanel(GroupData.Opened);
            }
        }
        else if (GroupData.Opening!=null)
        {
            IContext old = GroupData.Opening;
            OpenPanel(context);
            if (arg.OtherHandling.HasFlag(OtherToggleableGroupHandling.OtherClosesImmediately))
            {
                ClosePanelImmediate(old);
            }
            else
            {
                ClosePanel(old);
            }
        }
        else
        {
            OpenPanel(context);
        }
    }
    
    private void OnRequestOpenImmediate(EventArgs senderArgs,ToggleableRequestArgs arg)
    {
        IContext context = GroupDictionary[arg.MainID];
        if (GroupData.Opening == context) return; //Already appearing
        if (GroupData.Opened == context) return; //Already opened
        if (GroupData.Closing == context) return; //Means this key is hiding, for now this isn't allowed.
        
        if (GroupData.Opened != null)
        {
            if (arg.OtherHandling.HasFlag(OtherToggleableGroupHandling.WaitForOtherDisappear))
            {
                GroupData.WaitingForOpening = context;
            }
            if (arg.OtherHandling.HasFlag(OtherToggleableGroupHandling.OtherClosesImmediately))
            {
                ClosePanelImmediate(GroupData.Opened);
            }
            else
            {
                ClosePanel(GroupData.Opened);
            }
        }
        else
        {
            OpenPanelImmediate(context);
        }
    }
    
    private void OnRequestClose(EventArgs senderArgs)
    {
        if (GroupData.Opened == null) return;
        
        ClosePanel(GroupData.Opened);
    }
    
    private void OnRequestCloseImmediate(EventArgs senderArgs)
    {
        if (GroupData.Opened == null) return;
        
        ClosePanelImmediate(GroupData.Opened);
    }

    #region Shortcuts

    private void ClosePanel(IContext closed)
    {
        _closeCommand.Raise(closed.As<IEventContext>());
    }
    
    private void OpenPanel(IContext opened)
    {
        _openCommand.Raise(opened.As<IEventContext>());
    }
    private void OpenPanelImmediate(IContext opened)
    {
        ToggleableProcessMode mode = opened.As<IDataContext>().GetData<ToggleableDataSet>().OpeningMode;
        opened.As<IDataContext>().GetData<ToggleableDataSet>().OpeningMode = ToggleableProcessMode.Immediate;
        _openCommand.Raise(opened.As<IEventContext>());
        opened.As<IDataContext>().GetData<ToggleableDataSet>().OpeningMode = mode;
    }
    
    private void ClosePanelImmediate(IContext closed)
    {
        ToggleableProcessMode mode = closed.As<IDataContext>().GetData<ToggleableDataSet>().ClosingMode;
        closed.As<IDataContext>().GetData<ToggleableDataSet>().ClosingMode = ToggleableProcessMode.Immediate;
        _closeCommand.Raise(closed.As<IEventContext>());
        closed.As<IDataContext>().GetData<ToggleableDataSet>().ClosingMode = mode;
    }
    #endregion
    
}