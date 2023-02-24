using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKey",menuName = "Keys/Key")]
public class Key : ScriptableObject, ICreatableUnityAsset<Key>
{
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

    protected virtual void Awake()
    {
        AssetCreationEvents<Key>.NotifyCreate(this);
    }
}