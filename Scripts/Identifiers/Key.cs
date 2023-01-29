using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKey",menuName = "Keys/Key")]
public class Key : ScriptableObject
{
    public static event Action<Key> onCreate;
    public static event Action<Key> onDestroy; 
    protected virtual bool HideIDString => false;
    [SerializeField][HideIf("HideIDString")][OnValueChanged("OnIDChanged")] private string _id;

    protected virtual void OnIDChanged(string value)
    {
            
    }
        
    public virtual string ID
    {
        get => _id;
        set => _id = value;
    }

    private void Awake()
    {
        onCreate?.Invoke(this);
    }
}