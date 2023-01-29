using UnityEngine;

public interface IUnityComponent : IUnityObject
{
    public Transform transform { get; }
    public GameObject gameObject { get; }
    T GetComponent<T>();
    T[] GetComponents<T>();
}