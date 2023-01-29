using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Event_",menuName = "Keys/Event Key")]
public class EventKey : Key
{
    [SerializeField] private bool _isAsyncEvent;
    public bool IsAsyncEvent => _isAsyncEvent;
        
    [FormerlySerializedAs("_onlyGlobal")]
    [OnValueChanged("OnOnlyGlobalChanged")]
    [SerializeField] private bool _mustBeGlobal;
    public bool MustBeGlobal => _mustBeGlobal;

    public void SetupKey(string id,bool mustBeGlobal = false,string arg1Type = "",string arg2Type = "", string returnType = "",bool isAsyncEvent = false)
    {
        ID = id;
        _isAsyncEvent = isAsyncEvent;
        _arg1Type = arg1Type;
        _arg2Type = arg2Type;
        _returnType = returnType;
        _mustBeGlobal = mustBeGlobal;
            
#if UNITY_EDITOR
        ResetNameEditor();
#endif
    } 

    private void OnOnlyGlobalChanged(bool value)
    {
#if UNITY_EDITOR
        ResetNameEditor();
#endif
    }

#if UNITY_EDITOR
    private void SelectArg1Type()
    {
        TypeSelector selector = new TypeSelector(GetTypes(),false);
        selector.SelectionConfirmed += OnSelectArg1TypeConfirmed;
        selector.ShowInPopup();
    }

    private void OnSelectArg1TypeConfirmed(IEnumerable<Type> obj)
    {
        Type selectedType = obj.First();
        _arg1Type = selectedType.GetNiceName();
        ResetNameEditor();
    }

    private void SelectArg2Type()
    {
        TypeSelector selector = new TypeSelector(GetTypes(),false);
        selector.SelectionConfirmed += OnSelectArg2TypeConfirmed;
        selector.ShowInPopup();
    }

    private void OnSelectArg2TypeConfirmed(IEnumerable<Type> obj)
    {
        Type selectedType = obj.First();
        _arg2Type = selectedType.GetNiceName();
        ResetNameEditor();
    }

    private void SelectReturnType()
    {
        TypeSelector selector = new TypeSelector(GetTypes(),false);
        selector.SelectionConfirmed += OnSelectReturnTypeConfirmed;
        selector.ShowInPopup();
    }

    private void OnSelectReturnTypeConfirmed(IEnumerable<Type> obj)
    {
        Type selectedType = obj.First();
        _returnType = selectedType.GetNiceName();
        ResetNameEditor();
    }

    private static IEnumerable<Type> GetTypes()
    {
        List<Type> list = new List<Type>();
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (a.FullName.Contains("System.Object")) continue;
            if (a.FullName.Contains("Microsoft")) continue;
            if (a.FullName.Contains("Cecil")) continue;
            foreach (Type t in a.GetTypes())
            {
                if (t.IsNestedPrivate) continue;
                if (t.IsNotPublic) continue;
                list.Add(t);
            }
        }

        return list;
    }

    private void ResetNameEditor()
    {
        string assetPath = AssetDatabase.GetAssetPath(this);
        string globalSymbol = _mustBeGlobal ? "GlobalOnly_" : "";
        string firstArgSeparator = string.IsNullOrEmpty(_arg1Type) ? "" : "_";
        string secondArgSeparator = string.IsNullOrEmpty(_arg2Type) ? "" : "_";
        string returnArgSeparator = string.IsNullOrEmpty(_returnType) ? "" : "_";
        string starting = null;
        if (!IsAsyncEvent)
        {
            starting = (string.IsNullOrEmpty(_returnType) ? "AE_" : "AER_");
        }
        else
        {
            starting = "AE_Async_";
        }
            
        string newName = starting + globalSymbol + _arg1Type + firstArgSeparator + _arg2Type + secondArgSeparator + _returnType + returnArgSeparator + ID;
        name = newName;
        AssetDatabase.RenameAsset(assetPath, newName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

    private void OnArg1Changed(string value)
    {
    }
        
    [SerializeField][OnValueChanged("OnArg1Changed")][HideIf("IsAsyncEvent")]
    [InlineButton("SelectArg1Type"," ")] private string _arg1Type;
    public string Arg1Type => _arg1Type;
        
    private void OnArg2Changed(string value)
    {
    }
        
    [SerializeField][OnValueChanged("OnArg2Changed")][HideIf("IsAsyncEvent")]
    [InlineButton("SelectArg2Type"," ")] private string _arg2Type;
    public string Arg2Type => _arg2Type;
        
    private void OnReturnChanged(string value)
    {
    }
        
    [SerializeField][OnValueChanged("OnReturnChanged")][HideIf("IsAsyncEvent")]
    [InlineButton("SelectReturnType"," ")] private string _returnType;
    public string ReturnType => _returnType;

    [Button(ButtonSizes.Large)]
    public void UpdateNameToMatch()
    {
#if UNITY_EDITOR
        ResetNameEditor();
#endif
    }
}