using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Dragon.Core
{
    public static class EventKeyPath
    {
        public static void InitIfNot()
        {
            if (_IsInit) return;
            AssetCreationEvents<EventKey>.onCreate += SetRequireUpdate;
            AssetCreationEvents<EventKey>.onDestroy += SetRequireUpdate;
            _RequireUpdate = true;
            _IsInit = true;
        }

        public static void RequestUpdate()
        {
            _TypeToKeyDictionary = new Dictionary<Type, IEnumerable>();
            _RequireUpdate = true;
        }

        private static void SetRequireUpdate(Key obj)
        {
            _TypeToKeyDictionary = new Dictionary<Type, IEnumerable>();
            _RequireUpdate = true;
        }
    
#if UNITY_EDITOR
        public static List<EventKeyInfo> GetKeys()
        {
            InitIfNot();
            if (!RequireUpdate)
            {
                return Keys;
            }
        
            PathListConfig pathList = AssetDatabase.LoadAssetAtPath<PathListConfig>("Assets/_Project/Paths/EventKeyPaths.asset");
            if (pathList == null) return new List<EventKeyInfo>();
            Dictionary<string,IDPath> guidToPathDictionary = pathList.GetGUIDToPathDictionary("t:EventKey");

            List<EventKeyInfo> allKeys = new List<EventKeyInfo>();
            foreach (string guid in guidToPathDictionary.Keys)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                EventKey key = AssetDatabase.LoadAssetAtPath<EventKey>(assetPath);
                if (key == null) continue;
                allKeys.Add(new EventKeyInfo()
                {
                    PathID = guidToPathDictionary[guid].PathID,
                    Key = key,
                    Path = assetPath
                });
            }
            Update(allKeys);
        
            return Keys;
        }
#endif


        private static bool _IsInit;
        private static bool _RequireUpdate = true;
        public static bool RequireUpdate => _RequireUpdate;
        private static List<EventKeyInfo> _Keys;
        public static List<EventKeyInfo> Keys => _Keys;

        private static Dictionary<Type, IEnumerable> _TypeToKeyDictionary =
            new Dictionary<Type, IEnumerable>();
        public static Dictionary<Type, IEnumerable> TypeToKeyDictionary => _TypeToKeyDictionary;

        public static void Update(List<EventKeyInfo> keys)
        {
            _RequireUpdate = false;
            _Keys = keys;
        }
    }
}