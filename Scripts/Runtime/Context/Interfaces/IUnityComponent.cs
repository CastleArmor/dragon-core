using UnityEngine;

namespace Dragon.Core
{
    public interface IUnityComponent : IUnityObject
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }
        T GetComponent<T>();
        bool TryGetComponent<T>(out T comp);
        T[] GetComponents<T>();
    }
}