using System;

namespace Dragon.Core
{
    public static class AssetCreationEvents<T> where T : UnityEngine.Object,ICreatableUnityAsset<T>
    {
        public static event Action<T> onCreate;
        public static event Action<T> onDestroy;

        public static void NotifyCreate(T created)
        {
            onCreate?.Invoke(created);
        }
    }
}