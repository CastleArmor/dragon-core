using UnityEngine;

namespace Dragon.Core
{
    [CreateAssetMenu(fileName = "GOTag_",menuName = "Keys/GO Tag Key")]
    public class GOTagKey : TagKey,ICreatableUnityAsset<GOTagKey>
    {
        protected override void Awake()
        {
            AssetCreationEvents<GOTagKey>.NotifyCreate(this);
        }
    }
}