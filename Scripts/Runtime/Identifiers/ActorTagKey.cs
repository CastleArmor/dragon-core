using UnityEngine;

[CreateAssetMenu(fileName = "ATag_",menuName = "Keys/Actor Tag Key")]
public class ActorTagKey : Key,ICreatableUnityAsset<ActorTagKey>
{
    protected override void Awake()
    {
        AssetCreationEvents<ActorTagKey>.NotifyCreate(this);
    }
}