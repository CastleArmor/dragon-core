using UnityEngine;

namespace Dragon.Core
{
    [CreateAssetMenu(fileName = "AbilityKey_",menuName = "Keys/Ability Key")]
    public class AbilityKey : Key, ICreatableUnityAsset<AbilityKey>
    {
        protected override void Awake()
        {
            AssetCreationEvents<AbilityKey>.NotifyCreate(this);
        }
    }
}