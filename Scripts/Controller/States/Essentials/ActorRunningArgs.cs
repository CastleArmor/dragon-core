using System.Collections.Generic;
using UnityEngine;

public struct ActorRunningArgs
{
    public string UsageRequestID;
    public bool DoNotParentToUser;
    public GameObject PrefabOrInstance;
    public List<SlotOccupationInfo> OccupationInfos;
}