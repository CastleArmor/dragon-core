using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
#if UNITY_EDITOR
#endif

namespace Dragon.Core
{
    public struct NameGenericKeyPuller<T> where T : UnityEngine.Object,ICreatableUnityAsset<T>
    {
        public string TypeName;
        private static IEnumerable<ResourceNameAssetInfo<T>> GetAllResourceKeys() //filter like t:ClassName etc
        {
        
#if UNITY_EDITOR
            NameGenericKeyPathRegistry<T>.InitIfNot();
            if (!NameGenericKeyPathRegistry<T>.RequireUpdate)
            {
                return NameGenericKeyPathRegistry<T>.Keys;
            }
            PathListConfig pathList = AssetDatabase.LoadAssetAtPath<PathListConfig>("Assets/_Project/Paths/"+typeof(T).Name+"Paths.asset");
            Dictionary<string,IDPath> guidToPathDictionary = pathList.GetGUIDToPathDictionary("t:"+typeof(T).Name);

            List<ResourceNameAssetInfo<T>> allKeys = new List<ResourceNameAssetInfo<T>>();
            foreach (string guid in guidToPathDictionary.Keys)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T key = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (key == null) continue;
                allKeys.Add(new ResourceNameAssetInfo<T>()
                {
                    PathID = guidToPathDictionary[guid].PathID,
                    Key = key,
                    Path = assetPath
                });
            }
            NameGenericKeyPathRegistry<T>.Update(allKeys);
#endif
        
            return NameGenericKeyPathRegistry<T>.Keys;
        }

        private bool EvaluateForAppropriateResource(ResourceNameAssetInfo<T> keyInfo)
        {
#if UNITY_EDITOR
            if (keyInfo.Key == null) return false;
            return keyInfo.Key.name.Contains(TypeName);
#else
        return false;
#endif
        }
    
        private static ValueDropdownItem PrepareDropdownItem(ResourceNameAssetInfo<T> keyInfo)
        {
#if UNITY_EDITOR
            ValueDropdownItem item = new ValueDropdownItem();
            if (keyInfo.Path.StartsWith("Assets/_Project"))
            {
                item.Text = "P"+"/"+keyInfo.Key.name;
            }
            else if (keyInfo.Path.StartsWith("Assets/Framework"))
            {
                item.Text = "F"+"/"+keyInfo.Key.name;
            }
            else
            {
                item.Text = "?/"+keyInfo.Key.name;
            }
            item.Value = keyInfo.Key;
            return item;
#else
        return default;
#endif
        }
    
        public IEnumerable GetAllAppropriateKeys(string typeName)
        {
            TypeName = typeName;
#if UNITY_EDITOR
            return GetAllResourceKeys()
                .Where(EvaluateForAppropriateResource)
                .Select(PrepareDropdownItem);
#else
        return null;
#endif
        }
    }
}