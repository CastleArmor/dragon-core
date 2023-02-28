using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct IDPath
{
    public string PathID;
    [FolderPath]
    public string Path;
}

[CreateAssetMenu(fileName = "PathList_",menuName = "List/Path List")]
public class PathListConfig : ScriptableObject
{
    [SerializeField] private List<IDPath> _list;
    public List<IDPath> List
    {
        get => _list;
        set => _list = value;
    }

    public string[] GetPaths()
    {
        return _list.Select((a) => a.Path).ToArray();
    } 

#if UNITY_EDITOR
    
    public string[] EditorGetAssetGUIDs(string filters)
    {
        return AssetDatabase.FindAssets(filters, GetPaths());   
    }

    public Dictionary<string, IDPath> GetGUIDToPathDictionary(string filters)
    {
        Dictionary<string, IDPath> dictionary = new Dictionary<string, IDPath>();
        foreach (IDPath path in _list)
        {
            foreach (string guidsInPath in AssetDatabase.FindAssets(filters, new string[] {path.Path}))
            {
                dictionary.Add(guidsInPath,path);
            }
        }

        return dictionary;
    }
    
#endif
}