using System;
using UnityEngine;

public abstract class GlobalSingleton : MonoBehaviour
{
}
public abstract class GlobalSingleton<T> : GlobalSingleton where T : GlobalSingleton
{
    private static bool _isInstanceBeingDestroyed;
    public static bool IsInstanceBeingDestroyed => _isInstanceBeingDestroyed;
    private static bool _isAppQuit;
    public static bool IsAppQuit => _isAppQuit;
    private static T _instance;
    protected static T Instance
    {
        get
        {
            if (!Ensure()) return null;

            return _instance;
        }
    }

    protected static bool Ensure()
    {
        if (_isInstanceBeingDestroyed) return false;
        if (_instance == null)
        {
            GameObject go = new GameObject(typeof(T).Name+"-GlobalSingleton");
            go.AddComponent<T>();
            DontDestroyOnLoad(go);
        }

        return true;
    }

    private void OnApplicationQuit()
    {
        _isAppQuit = true;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        _isInstanceBeingDestroyed = false;
        _instance = this as T;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _isInstanceBeingDestroyed = true;
            _instance = null;
        }
    }
    
} 