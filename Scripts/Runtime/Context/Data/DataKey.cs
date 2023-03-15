using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif
#if UNITY_EDITOR
#endif

namespace Dragon.Core
{
    [CreateAssetMenu(fileName = "NewDataKey",menuName = "Keys/Data Key")]
    public class DataKey : Key,ICreatableUnityAsset<DataKey>
    {
        protected override void Awake()
        {
            AssetCreationEvents<DataKey>.NotifyCreate(this);
        }

        [SerializeField] private bool _fullTypeName;
#if UNITY_EDITOR
        [Button]
        private void SelectDataSetType()
        {
            TypeSelector selector = new TypeSelector(GetTypes(),false);
            selector.SelectionConfirmed += OnSelectionConfirmed;
            selector.ShowInPopup();
        }

        private void OnSelectionConfirmed(IEnumerable<Type> obj)
        {
            Type selectedType = obj.First();
            if (_fullTypeName)
            {
                _dataType = selectedType.AssemblyQualifiedName;
            }
            else
            {
                _dataType = selectedType.GetNiceName();
            }
            ResetNameEditor();
        }

        private static IEnumerable<Type> GetTypes()
        {
            List<Type> list = new List<Type>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.Namespace == "System")
                    {
                        list.Add(t);
                    }
                    else if (t.Namespace == "UnityEngine")
                    {
                        list.Add(t);
                    }
                    else if (typeof(IData).IsAssignableFrom(t))
                    {
                        list.Add(t);
                    }
                }
            }

            return list;
        }
#endif

        [SerializeField][TextArea] private string _dataType;
        public string DataType => _dataType;

        public void SetupKey(Type dataSetType,string givenName)
        {
            ID = givenName;
            
#if UNITY_EDITOR
            _dataType = dataSetType.GetNiceName();
            ResetNameEditor();
#endif
        }

#if UNITY_EDITOR
        private void ResetNameEditor()
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            string firstArgSeparator = string.IsNullOrEmpty(_dataType) ? "" : "_";
            //string secondArgSeparator = string.IsNullOrEmpty(ID) ? "" : "_";
            string starting = "DK_";
            
            string[] types = _dataType.Split(';');
            string firstType = types[0];
            if (_fullTypeName)
            {
                firstType = Type.GetType(firstType).GetNiceName();
            }
            string newName = starting + firstType + firstArgSeparator + ID;
            newName = newName.Replace('<', '[');
            newName = newName.Replace('>', ']');
            name = newName;
            AssetDatabase.RenameAsset(assetPath, newName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
        [Button(ButtonSizes.Large)]
        public void UpdateNameToMatch()
        {
#if UNITY_EDITOR
            ResetNameEditor();
#endif
        }

        public static DataKey CreateAtFolder<T>(string keyName)
        {
#if UNITY_EDITOR
            ScriptableObject obj = ScriptableObject.CreateInstance(typeof(DataKey));
            DataKey key = obj as DataKey;
            key.SetupKey(typeof(T),keyName);
            DEditorPath.EnsurePathExistence("Assets/_Project/ScriptAssets/DataKeys");
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/_Project/ScriptAssets/DataKeys" + "/" + key.name + ".asset");
            AssetDatabase.CreateAsset(key,uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return key;
#else
        return null;
#endif
        }
    }
}