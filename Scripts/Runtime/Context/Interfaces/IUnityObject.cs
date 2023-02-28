using System;

public interface IUnityObject : ICastable
{ 
    int GetInstanceID();
    public string name { get; set; }
}

public interface ICreatableUnityAsset<T> : IUnityObject where T : UnityEngine.Object
{
    public static event Action<T> onCreate;
    public static event Action<T> onDestroy; 
}