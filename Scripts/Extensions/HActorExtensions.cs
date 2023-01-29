using System;
using UnityEngine;

public static class HActorExtensions
{
    public static IActor StartTransientUsedMain(
        this IActor contextActor, 
        GameObject prefab,
        out Transform oldParent,
        bool setParentOnEnter = true, 
        bool snapPosRotToParent = true, 
        Action<IActor> onBeforeStartActor = null)
    {
        GameObject instance = prefab.IsPrefab() ? contextActor.GOPool.Retrieve(prefab).gameObject : prefab;
        IActor actor = instance.GetComponent<IActor>();
        actor.DataContext.ParentContext = contextActor.DataContext;
        //actor.ConfiguredUpdateHandler = contextActor.ConfiguredUpdateHandler;
        oldParent = actor.transform.parent;
        if (setParentOnEnter)
        {
            actor.transform.SetParent(contextActor.transform);
            if (snapPosRotToParent)
            {
                actor.transform.localPosition = Vector3.zero;
                actor.transform.localRotation = Quaternion.identity;
            }
        }
        actor.InitializeIfNot();
        onBeforeStartActor?.Invoke(actor);
        actor.BeginIfNot();
        return actor;
    }
}