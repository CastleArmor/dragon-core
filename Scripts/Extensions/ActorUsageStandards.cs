using System;
using System.Collections.Generic;
using UnityEngine;

public enum SlotOccupationMode
{
    Override, //Whatever is on the slot gets removed, and new one gets added.
    Additive //Added on on top, but when override comes along, it will be removed.
}

public struct SlotOccupationInfo
{
    public Key Slot;
    public SlotOccupationMode Mode;
}

public static class ActorUsageStandards
{
    public static ActorRunResult TryStartChildActor(
        IActor parent, 
        IGOInstance prefabOrInstance,
        string usageRequestID = "Default",
        DelegatedObject<bool> delegatedObject = null,
        Action<ActorUsageValidateArgs> evaluateCanStart = null,
        Action<ActorUsageEventArgs> startConfirmed = null,
        Dictionary<string,List<IActor>> runningDictionary = null,
        UniqueList<IActor> runningList = null,
        Action<IActor> finishEnded = null, 
        Action<IActor> cancelEnded = null,
        Action<IActor> beforeStart = null,
        bool doNotParentToUser = false,
        List<SlotOccupationInfo> occupationList = null,
        Dictionary<string,List<IActor>> occupationDictionary = null,
        Dictionary<IActor,List<string>> occupierToOccupiedList = null,
        InstancePool<List<IActor>> actorListPool = null,
        InstancePool<List<string>> stringListPool = null)
    {
        if(parent.IsBeingDestroyed) return new ActorRunResult(){IsSuccess = false};

        bool isPrefab = prefabOrInstance.gameObject.IsPrefab();
        IActor instanceActor = null;
        if (!isPrefab)
        {
            instanceActor = prefabOrInstance.GetComponent<IActor>();
            instanceActor.InitializeIfNot();   
        }

        //Validate
        bool validated = true;
        if (delegatedObject != null && evaluateCanStart != null)
        {
            delegatedObject.DelegateObject = true;
            evaluateCanStart.Invoke(new ActorUsageValidateArgs()
            {
                UsageRequestID = usageRequestID,
                PrefabOrInstance = prefabOrInstance,
                DelegateObject = delegatedObject
            });
            validated = delegatedObject.DelegateObject;
        }
        
        if (!validated)
        {
            Debug.Log("Not validated.");
            return new ActorRunResult(){IsSuccess = false};
        }

        var instance = isPrefab ? parent.GOPool.Retrieve(prefabOrInstance.gameObject) : prefabOrInstance;
        if (isPrefab)
        {
            instanceActor = instance.GetComponent<IActor>();
        }

        if (instanceActor is IUpdateOwner uoi && parent is IUpdateOwner uop)
        {
            uoi.ConfiguredUpdateHandler = uop.ConfiguredUpdateHandler; //Child shares update handlers.
        }
        
        if (occupationList != null && occupationDictionary != null && occupierToOccupiedList != null)
        {
            foreach (SlotOccupationInfo info in occupationList)
            {
                if (occupationDictionary.ContainsKey(info.Slot.ID))
                {
                    switch (info.Mode)
                    {
                        case SlotOccupationMode.Additive:
                            occupationDictionary[info.Slot.ID].Add(instanceActor);
                            break;
                        case SlotOccupationMode.Override:
                            for (var i = 0; i < occupationDictionary[info.Slot.ID].Count; i++)
                            {
                                var cancelled = occupationDictionary[info.Slot.ID][i];
                                cancelled.Cancel(usageRequestID);
                                occupationDictionary[info.Slot.ID].Remove(cancelled);
                                i--;
                            }
                            occupationDictionary[info.Slot.ID].Add(instanceActor);
                            break;
                    }
                }
                else
                {
                    occupationDictionary.Add(info.Slot.ID,actorListPool.Get());
                }
            }
            occupierToOccupiedList.Add(instanceActor,stringListPool.Get());
        }

        instanceActor.DataContext.ParentContext = parent.DataContext;
        instanceActor.InitializeIfNot();
        
        startConfirmed?.Invoke(new ActorUsageEventArgs()
        {
            ActorInstance = instanceActor,
            PrefabOrInstance = prefabOrInstance,
            UsageRequestID = usageRequestID
        });

        if (runningDictionary != null)
        {
            if (runningDictionary.ContainsKey(prefabOrInstance.ObjectTypeID))
            {
                runningDictionary[prefabOrInstance.ObjectTypeID].Add(instanceActor);
            }
            else
            {
                runningDictionary.Add(prefabOrInstance.ObjectTypeID,new List<IActor>(){instanceActor});
            }
        }
        
        IActor actor = instanceActor;

        if (cancelEnded != null)
        {
            actor.onCancelEnded += cancelEnded;
        }
        if (finishEnded != null)
        {
            actor.onFinishEnded += finishEnded;
        }
        
        if (!doNotParentToUser)
        {
            actor.transform.SetParent(parent.transform);
            actor.transform.localEulerAngles = Vector3.zero;
            actor.transform.localPosition = Vector3.zero;
        }

        if (runningList != null)
        {
            runningList.Add(actor);
        }
        
        beforeStart?.Invoke(actor);
        actor.BeginIfNot();
        Debug.Log("Starting actor");
        return new ActorRunResult()
        {
            IsSuccess = true,
            RunningInstance = actor
        };
    }

    public static void StandardChildActorRelease(IActor endedActor,
        Dictionary<string,List<IActor>> runningDictionary = null, 
        UniqueList<IActor> runningList = null,
        Action<IActor> finishEnded = null, 
        Action<IActor> cancelEnded = null,
        Dictionary<string,List<IActor>> occupationDictionary = null,
        Dictionary<IActor,List<string>> occupierToOccupiedList = null,
        InstancePool<List<IActor>> actorListPool = null,
        InstancePool<List<string>> stringListPool = null)
    {
        endedActor.onCancelEnded -= cancelEnded;
        endedActor.onFinishEnded -= finishEnded;

        if (occupationDictionary != null && occupierToOccupiedList != null && actorListPool != null && stringListPool != null)
        {
            foreach (string occupiedSlot in occupierToOccupiedList[endedActor])
            {
                if (occupationDictionary.ContainsKey(occupiedSlot))
                {
                    occupationDictionary[occupiedSlot].Remove(endedActor);
                }

                if (occupationDictionary[occupiedSlot].Count == 0)
                {
                    actorListPool.Return(occupationDictionary[occupiedSlot]);
                }
            }

            var list = occupierToOccupiedList[endedActor];
            stringListPool.Return(list);
            occupierToOccupiedList.Remove(endedActor);
        }
        
        runningList?.Remove(endedActor);
        if (runningDictionary != null)
        {
            runningDictionary[endedActor.ObjectTypeID].Remove(endedActor);
            if (runningDictionary[endedActor.ObjectTypeID].Count == 0)
            {
                runningDictionary.Remove(endedActor.ObjectTypeID);
            }
        }
    }

    public static void StopAllChildActors(string cancelID,UniqueList<IActor> runningList)
    {
        for (var i = 0; i < runningList.Count; i++)
        {
            runningList[i].Cancel(cancelID);
            i--; //We expect cancelling triggering the StandardChildActorRelease resulting in runningList element removals.
        }
    }
}