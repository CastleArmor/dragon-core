using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public class CoreActor : Actor,ITagOwner
{
    [SerializeField][ReadOnly] private MonoBehaviour _tagOwnerObject;
    private ITagOwner _tagOwner;
    [SerializeField] private MonoActorState _runningState;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (Application.isPlaying) return;
        if (_tagOwnerObject != null) return;
        foreach (ITagOwner owner in GetComponents<ITagOwner>())
        {
            if (owner == (ITagOwner) this) continue;
            _tagOwnerObject = owner as MonoBehaviour;
        }
    }

    protected override void OnBeforeContextsInitialize()
    {
        base.OnBeforeContextsInitialize();
        if (_tagOwnerObject)
        {
            _tagOwner = _tagOwnerObject.As<ITagOwner>();
            _tagOwner.As<IInitializable>().InitializeIfNot();
        }
    }

    protected override void OnAfterContextsInitialized()
    {
        base.OnAfterContextsInitialized();
        if (_runningState is IInitializedSubState state)
        {
            state.Initialize();
        }
    }

    protected override void OnBeginLogic()
    {
        base.OnBeginLogic();
        _runningState.CheckoutEnter(this);
    }

    protected override void OnStopLogic()
    {
        base.OnStopLogic();
        _runningState.CheckoutExit();
    }

    public event Action<ITagOwner, string> onTagAdded
    {
        add => _tagOwner.onTagAdded += value;
        remove => _tagOwner.onTagAdded -= value;
    }

    public event Action<ITagOwner, string> onTagRemoved
    {
        add => _tagOwner.onTagRemoved += value;
        remove => _tagOwner.onTagRemoved -= value;
    }

    public event Action<ITagOwner, string, bool> onTagChanged
    {
        add => _tagOwner.onTagChanged += value;
        remove => _tagOwner.onTagChanged -= value;
    }

    public bool ContainsTag(string t)
    {
        return _tagOwner.ContainsTag(t);
    }

    public void AddTag(string t)
    {
        _tagOwner.AddTag(t);
    }

    public void RemoveTag(string t)
    {
        _tagOwner.RemoveTag(t);
    }
}