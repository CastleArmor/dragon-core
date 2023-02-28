using UnityEngine;

[CreateAssetMenu(fileName = "UK_",menuName = "Keys/Update Key")]
public class UpdateKey : Key,ICreatableUnityAsset<UpdateKey>
{
    protected override void Awake()
    {
        AssetCreationEvents<UpdateKey>.NotifyCreate(this);
    }
}