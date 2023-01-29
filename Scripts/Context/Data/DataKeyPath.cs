using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//
public static class DataKeyPath
{
#if UNITY_EDITOR
    public static List<DataKeyInfo> GetKeys()
    {
        InitIfNot();
        if (!RequireUpdate)
        {
            return Keys;
        }
        
        PathListConfig pathList = AssetDatabase.LoadAssetAtPath<PathListConfig>("Assets/_Project/Paths/DataKeyPaths.asset");
        if (pathList == null) return null;
        Dictionary<string,IDPath> guidToPathDictionary = pathList.GetGUIDToPathDictionary("t:DataKey");

        List<DataKeyInfo> allKeys = new List<DataKeyInfo>();
        foreach (string guid in guidToPathDictionary.Keys)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            DataKey key = AssetDatabase.LoadAssetAtPath<DataKey>(assetPath);
            if (key == null) continue;
            allKeys.Add(new DataKeyInfo()
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
    
    public static void InitIfNot()
    {
        if (_IsInit) return;
        _Keys = new List<DataKeyInfo>();
        Key.onCreate += SetRequireUpdate;
        Key.onDestroy += SetRequireUpdate;
        _RequireUpdate = true;
        _IsInit = true;
    }

    private static void SetRequireUpdate(Key obj)
    {
        _TypeToKeyDictionary = new Dictionary<Type, IEnumerable>();
        _RequireUpdate = true;
    }

    private static bool _IsInit;
    private static bool _RequireUpdate = true;
    public static bool RequireUpdate => _RequireUpdate || _Keys == null;
    private static List<DataKeyInfo> _Keys;
    public static List<DataKeyInfo> Keys => _Keys;

    private static Dictionary<Type, IEnumerable> _TypeToKeyDictionary =
        new Dictionary<Type, IEnumerable>();
    public static Dictionary<Type, IEnumerable> TypeToKeyDictionary => _TypeToKeyDictionary;

    public static void Update(List<DataKeyInfo> keys)
    {
        _RequireUpdate = false;
        _Keys = keys;
    }
}