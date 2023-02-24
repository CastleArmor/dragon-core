using System;
using UnityEngine;

[System.Serializable]
public struct ActorUsageValidateArgs
{
    public string UsageRequestID;
    public IGOInstance PrefabOrInstance;
    public DelegatedObject<bool> DelegateObject;
}

[System.Serializable]
public struct ActorUsageEventArgs
{
    public string UsageRequestID;
    public IGOInstance PrefabOrInstance;
    public IActor ActorInstance;
}

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

    public static IActor GetActor(this Collider collider)
    {
        if (collider.TryGetComponent(out IActor actor))
        {
            return actor;
        }
        else
        {
            if (collider.TryGetComponent(out ActorPointer pointer))
            {
                return pointer.Pointed;
            }
        }

        return null;
    }

    public static bool ContainsTag(this IActor actor, Key tagKey)
    {
        return actor.As<ITagOwner>().ContainsTag(tagKey.ID);
    }

    public static bool TryAddTag(this IActor actor, Key tagKey)
    {
        if (actor.ContainsTag(tagKey)) return false;
        actor.As<ITagOwner>().AddTag(tagKey.ID);
        return true;
    }

    public static bool TryRemoveTag(this IActor actor, Key tagKey)
    {
        if (!actor.ContainsTag(tagKey)) return false;
        actor.As<ITagOwner>().RemoveTag(tagKey.ID);
        return true;
    }

    public static void AddTag(this IActor actor, Key tagKey)
    {
        actor.As<ITagOwner>().AddTag(tagKey.ID);
    }
    
    public static void RemoveTag(this IActor actor, Key tagKey)
    {
        actor.As<ITagOwner>().RemoveTag(tagKey.ID);
    }
}