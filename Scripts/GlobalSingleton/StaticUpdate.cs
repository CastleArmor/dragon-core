using System;
using System.Collections;
using UnityEngine;



public class StaticUpdate : GlobalSingleton<StaticUpdate>
{
    public static event Action<bool> onApplicationPause
    {
        add => Instance._onApplicationPause += value;
        remove => Instance._onApplicationPause -= value;
    }
    public static event Action onApplicationQuit
    {
        add => Instance._onApplicationQuit += value;
        remove => Instance._onApplicationQuit -= value;
    }
    public static event Action onUpdate
    {
        add => Instance._onUpdate += value;
        remove => Instance._onUpdate -= value;
    }
    public static event Action onFixedUpdate
    {
        add => Instance._onFixedUpdate += value;
        remove => Instance._onFixedUpdate -= value;
    }
    public static event Action onLateUpdate
    {
        add => Instance._onLateUpdate += value;
        remove => Instance._onLateUpdate -= value;
    }

    public event Action<bool> _onApplicationPause; 
    public event Action _onApplicationQuit;
    public event Action _onUpdate;
    public event Action _onFixedUpdate;
    public event Action _onLateUpdate;

    public static Coroutine StartCoroutineStatic(IEnumerator enumerator)
    {
        return Instance.StartCoroutine(enumerator);
    }
    private void Update()
    {
        _onUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        _onFixedUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        _onLateUpdate?.Invoke();
    }

    private void OnApplicationQuit()
    {
        _onApplicationQuit?.Invoke();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        _onApplicationPause?.Invoke(pauseStatus);
    }
}