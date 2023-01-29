using System;
using UnityEngine;

public abstract class GlobalSingleton : MonoBehaviour
{
}
public abstract class GlobalSingleton<T> : GlobalSingleton where T : GlobalSingleton
{
    private static T _instance;
    protected static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name+"-GlobalSingleton");
                go.AddComponent<T>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        _instance = this as T;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    
} 