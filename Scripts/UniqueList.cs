using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class UniqueList<T>
{
    [SerializeField]
    private string _listDebugName;
    [ShowInInspector] [ReadOnly] private readonly List<T> _list = new List<T>();
    private readonly HashSet<T> _set = new HashSet<T>();
    public List<T> List => _list;

    public event Action<UniqueList<T>, T> onAdded;
    public event Action<UniqueList<T>, T> onRemoved;
    public event Action<UniqueList<T>> onChanged;

    public int Count => _list.Count;

    public T this[int i]
    {
        get => _list[i];
        set => _list[i] = value;
    }

    public void TryAdd(T element)
    {
        if (_set.Contains(element))
        {
            return;
        }
        Add(element);
    }

    public void RemoveAt(int i)
    {
        T element = _list[i];
        _list.Remove(element);
        _set.Remove(element);
        onRemoved?.Invoke(this, element);
        onChanged?.Invoke(this);
    }

    public void Add(T element)
    {
        if (_set.Contains(element))
        {
            Debug.LogError("You've already added main with name " + element +
                           " to " + _listDebugName + " list, something is wrong with your logic");
            return;
        }

        _set.Add(element);
        _list.Add(element);
        onAdded?.Invoke(this, element);
        onChanged?.Invoke(this);
    }

    public bool Contains(T element)
    {
        return _set.Contains(element);
    }

    public void Clear()
    {
        _list.Clear();
        _set.Clear();
        onChanged?.Invoke(this);
    }

    public void Remove(T element)
    {
        if (!_set.Contains(element))
        {
            Debug.LogError("Main with name " + element +
                           " is already doesn't exist in " + _listDebugName + " UniqueList<" + typeof(T).Name + ">");
            return;
        }

        _set.Remove(element);
        _list.Remove(element);
        onRemoved?.Invoke(this, element);
        onChanged?.Invoke(this);
    }
}