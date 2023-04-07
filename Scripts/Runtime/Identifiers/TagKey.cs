using UnityEngine;

namespace Dragon.Core
{
    [CreateAssetMenu(fileName = "Tag_",menuName = "Keys/Tag Key")]
    public class TagKey : Key,ICreatableUnityAsset<TagKey>
    {
        protected override void Awake()
        {
            AssetCreationEvents<TagKey>.NotifyCreate(this);
        }
    }
}