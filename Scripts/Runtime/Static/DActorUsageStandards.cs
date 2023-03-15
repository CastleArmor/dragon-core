using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public static class DActorUsageStandards
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
            InstancePool<List<string>> stringListPool = null,
            string relationKey = null,bool doNotMoveToParent = false)
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
        
            if (occupationList != null && occupationDictionary != null && occupierToOccupiedList != null)
            {
                Debug.Log("Slotting");
                occupierToOccupiedList.Add(instanceActor,stringListPool.Get());
                foreach (SlotOccupationInfo info in occupationList)
                {
                    Debug.Log("Evaluating " + info.Slot.ID + " - " + info.Mode);
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
                                    cancelled.CancelIfNotEnded(usageRequestID);
                                    Debug.Log("Cancelled " + cancelled.name);
                                    //occupationDictionary[info.Slot.ID].Remove(cancelled);
                                    i--;
                                }
                                occupationDictionary[info.Slot.ID].Add(instanceActor);
                                break;
                        }
                    }
                    else
                    {
                        occupationDictionary.Add(info.Slot.ID,actorListPool.Get());
                        occupationDictionary[info.Slot.ID].Add(instanceActor);
                    }
                    occupierToOccupiedList[instanceActor].Add(info.Slot.ID);
                }
            }

            instanceActor.DataContext.ParentContext = parent.DataContext;
            if (!string.IsNullOrEmpty(relationKey))
            {
                string relationStringKey = "ActorRelation" + instanceActor.DataContext.ContextID;
                parent.DataContext.SetData<string>(relationStringKey,instanceActor.DataContext.ContextID);
                parent.DataContext.SetData<IContext>(relationKey,instanceActor.DataContext);
            }
            instanceActor.InitializeIfNot();

            instanceActor.DataContext.SetData(parent.DataContext.GetData<IConfiguredUpdateBehaviour>());
        
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
            Debug.Log("Starting actor " + actor.name);
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
                        occupationDictionary[occupiedSlot].Clear();
                        actorListPool.Return(occupationDictionary[occupiedSlot]);
                    }
                }

                var list = occupierToOccupiedList[endedActor];
                list.Clear();
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

            IContext parent = endedActor.DataContext.ParentContext;
            string relationStringKey = "ActorRelation" + endedActor.DataContext.ContextID;
            if (parent.ContainsData<string>(relationStringKey))
            {
                string relationKey = parent.GetData<string>(relationStringKey);
                endedActor.DataContext.ParentContext.RemoveData<IContext>(relationKey);
                endedActor.DataContext.ParentContext.RemoveData<string>(relationStringKey);
            }
        }

        public static void StopAllChildActors(string cancelID,UniqueList<IActor> runningList)
        {
            for (var i = 0; i < runningList.Count; i++)
            {
                runningList[i].CancelIfNotEnded(cancelID);
                i--; //We expect cancelling triggering the StandardChildActorRelease resulting in runningList element removals.
            }
        }
    }
}