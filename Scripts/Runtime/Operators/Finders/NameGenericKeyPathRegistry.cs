using System;
using System.Collections;
using System.Collections.Generic;

namespace Dragon.Core
{
    public static class NameGenericKeyPathRegistry<T> where T : UnityEngine.Object,ICreatableUnityAsset<T>
    {
        public static void InitIfNot()
        {
            if (_IsInit) return;
            AssetCreationEvents<T>.onCreate += SetRequireUpdate;
            AssetCreationEvents<T>.onDestroy += SetRequireUpdate;
            _RequireUpdate = true;
            _IsInit = true;
        }

        private static void SetRequireUpdate(T obj)
        {
            _TypeToKeyDictionary = new Dictionary<Type, IEnumerable>();
            _RequireUpdate = true;
        }

        private static bool _IsInit;
        private static bool _RequireUpdate = true;
        public static bool RequireUpdate => _RequireUpdate;
        private static List<ResourceNameAssetInfo<T>> _Keys;
        public static List<ResourceNameAssetInfo<T>> Keys => _Keys;

        private static Dictionary<Type, IEnumerable> _TypeToKeyDictionary =
            new Dictionary<Type, IEnumerable>();
        public static Dictionary<Type, IEnumerable> TypeToKeyDictionary => _TypeToKeyDictionary;

        public static void Update(List<ResourceNameAssetInfo<T>> keys)
        {
            _RequireUpdate = false;
            _Keys = keys;
        }
    }
}