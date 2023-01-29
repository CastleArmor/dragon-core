using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct ToggleableRequestArgs
{
    public string MainID;
    public OtherToggleableGroupHandling OtherHandling;
}

[Flags]
public enum OtherToggleableGroupHandling
{
    None,
    WaitForOtherDisappear,
    OtherClosesImmediately
}

[System.Serializable]
public class ToggleableGroupDataSet : InstalledData
{
    [SerializeField] private bool _openFirstMemberAtEnter = true;
    public bool OpenFirstMemberAtEnter
    {
        get => _openFirstMemberAtEnter;
        set => _openFirstMemberAtEnter = value;
    }
    
    public void AddToggleable(GameObject toggleableGO)
    {
        _toggleables.Add(toggleableGO);
        onAddedToggleable?.Invoke(toggleableGO);
    }

    public void RemoveToggleable(GameObject toggleableGO)
    {
        _toggleables.Remove(toggleableGO);
        onRemoveToggleable?.Invoke(toggleableGO);
    }

    public event Action<GameObject> onAddedToggleable;
    public event Action<GameObject> onRemoveToggleable;

    [FormerlySerializedAs("_initialToggleables")] 
    [SerializeField] private List<GameObject> _toggleables;
    public List<GameObject> Toggleables => _toggleables;

    [ShowInInspector][ReadOnly]
    private IContext _opened;
    public IContext Opened
    {
        get => _opened;
        set => _opened = value;
    }
    
    [ShowInInspector][ReadOnly]
    private IContext _opening; 
    public IContext Opening 
    {
        get => _opening;
        set => _opening = value;
    }
    
    [ShowInInspector][ReadOnly]
    private IContext _closing; 
    public IContext Closing 
    {
        get => _closing;
        set => _closing = value;
    }
    
    [ShowInInspector][ReadOnly]
    private IContext _waitingForOpening; 
    public IContext WaitingForOpening 
    {
        get => _waitingForOpening;
        set => _waitingForOpening = value;
    }
    
    [ShowInInspector][ReadOnly]
    private IContext _waitingForImmediateOpening; 
    public IContext WaitingForImmediateOpening 
    {
        get => _waitingForImmediateOpening;
        set => _waitingForImmediateOpening = value;
    }
    
    [ShowInInspector][ReadOnly]
    private IContext _previous;
    public IContext Previous
    {
        get => _previous;
        set => _previous = value;
    }
}