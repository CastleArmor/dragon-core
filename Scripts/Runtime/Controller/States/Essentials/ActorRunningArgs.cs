using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public struct ActorRunningArgs
    {
        public string UsageRequestID;
        public bool DoNotMoveToParent;
        public bool DoNotParentToUser;
        public GameObject PrefabOrInstance;
        public List<SlotOccupationInfo> OccupationInfos;
        public DataKey RelationKey;
    }
}