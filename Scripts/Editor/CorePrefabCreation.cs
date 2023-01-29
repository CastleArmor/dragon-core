using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CorePrefabCreation
{
    [MenuItem("GameObject/Actor/Core Instance",false,1)]
    public static void CreateInstance_ActorCoreInstance(MenuCommand cmd)
    {
        Selection.activeObject = CreateInstance("Assets/Framework/dragon-core/Prefabs/Actor.prefab");
    }
    [MenuItem("GameObject/Actor/Panel Instance",false,1)]
    public static void CreateInstance_ActorPanelInstance(MenuCommand cmd)
    {
        Selection.activeObject = CreateInstance("Assets/Framework/dragon-core/Prefabs/Actor-Panel.prefab");
    }
    
    public static GameObject CreateInstance(string path)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        Transform selectionTransform = Selection.activeTransform;
        GameObject instance = GameObject.Instantiate(prefab,selectionTransform);
        instance.name = prefab.name;
        return instance;
    }
}