using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IGOInstancePool
{
    public void Initialize(IGOInstance original, int initialCount, Transform parent);
    public void RegisterInstance(IGOInstance instance);
    public void UnregisterInstance(IGOInstance instance);
    public void Return(IGOInstance instance);
    public IGOInstance Retrieve();
}

[System.Serializable]
public class GOInstancePool : IGOInstancePool
{
    [ShowInInspector][ReadOnly] private Transform _containingParent;
    public Transform ContainingParent => _containingParent;
    [ShowInInspector][ReadOnly] private IGOInstance _original;
    public IGOInstance Original => _original;
    [ShowInInspector][ReadOnly] private List<IGOInstance> _activeInstances = new List<IGOInstance>();
    public List<IGOInstance> ActiveInstances => _activeInstances;
    [ShowInInspector][ReadOnly] private List<IGOInstance> _deactiveInstances = new List<IGOInstance>();
    public List<IGOInstance> DeactiveInstances => _deactiveInstances;
    [ShowInInspector][ReadOnly] private List<IGOInstance> _allInstances = new List<IGOInstance>();
    public List<IGOInstance> AllInstances => _allInstances;

    public void Initialize(IGOInstance original, int initialCount, Transform parent)
    {
        GameObject newPoolParent = new GameObject(original.ObjectTypeID + "-Pool");
        newPoolParent.transform.SetParent(parent);
        _containingParent = newPoolParent.transform;
        _original = original;
        Clone(initialCount);
    }

    public void RegisterInstance(IGOInstance instance)
    {
        instance.name = instance.name + "-P[" + _allInstances.Count + "]";
        _deactiveInstances.Add(instance);
        _allInstances.Add(instance);
    }

    public void UnregisterInstance(IGOInstance instance)
    {
        //If something goes wrong here, return the thing first
        _deactiveInstances.Remove(instance);
        _allInstances.Remove(instance);
    }

    public void Return(IGOInstance instance)
    {
        _activeInstances.Remove(instance);
        _deactiveInstances.Add(instance);
        instance.PoolCheckoutReturnedToPool();
    }

    public IGOInstance Retrieve()
    {
        if (_deactiveInstances.Count == 0)
        {
            Clone(1);
        }
       
        IGOInstance returned = _deactiveInstances[0];
        _deactiveInstances.RemoveAt(0);
        _activeInstances.Add(returned);
        returned.PoolCheckoutRetrievedFromPool();
        return returned;
    }

    private void Clone(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newInstance = GameObject.Instantiate(_original.gameObject);
            newInstance.name = newInstance.name.Replace("(Clone)", "-P[" + _allInstances.Count + "]");
            IGOInstance instance = newInstance.GetComponent<IGOInstance>();
            _deactiveInstances.Add(instance);
            _allInstances.Add(instance);
            instance.PoolCheckoutRegisteredToPool(this);
        }
    }
}