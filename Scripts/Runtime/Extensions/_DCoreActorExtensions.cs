using System;
using UnityEngine;

namespace Dragon.Core
{
    public static class _DColliderExtensions
    {
        public static bool GetFinalPointedColliderOwner(this Collider collider,out IActor owner, out Transform ownerTransform)
        {
            bool isFound = false;
            ownerTransform = collider.transform;
            isFound = collider.TryGetComponent(out owner);

            return isFound;
        }
    }
    
    public static class _DCoreActorExtensions
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
            actor.pContext.ParentContext = contextActor.pContext;
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

        public static bool ContainsTag(this IActor actor, ActorTagKey tagKey)
        {
            return actor.As<ITagOwner>().ContainsTag(tagKey.ID);
        }

        public static bool TryAddTag(this IActor actor, ActorTagKey tagKey)
        {
            if (actor.ContainsTag(tagKey)) return false;
            actor.As<ITagOwner>().AddTag(tagKey.ID);
            return true;
        }

        public static bool TryRemoveTag(this IActor actor, ActorTagKey tagKey)
        {
            if (!actor.ContainsTag(tagKey)) return false;
            actor.As<ITagOwner>().RemoveTag(tagKey.ID);
            return true;
        }

        public static void AddTag(this IActor actor, ActorTagKey tagKey)
        {
            actor.As<ITagOwner>().AddTag(tagKey.ID);
        }
    
        public static void RemoveTag(this IActor actor, ActorTagKey tagKey)
        {
            actor.As<ITagOwner>().RemoveTag(tagKey.ID);
        }
    
        /// <summary>
        /// O(1) time. Fast.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static Transform GetFinalLookTransform(this IActor actor)
        {
            return actor.pContext.GetData<D_Positioning>().FinalLookTransform;
        }

        /// <summary>
        /// O(1) time. Fast.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static Transform GetFinalFacingTransform(this IActor actor)
        {
            return actor.pContext.GetData<D_Positioning>().FinalFacingTransform;
        }

        public static IActor GetRoot(this IActor actor)
        {
            return actor.pContext.RootContext.GetData<IActor>();
        }
    
        public static IActor GetParent(this IActor actor)
        {
            return actor.pContext.ParentContext.GetData<IActor>();
        }
    
        /// <summary>
        /// O(1) time. Fast.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static Transform GetFinalMoveTransform(this IActor actor)
        {
            return actor.pContext.GetData<D_Positioning>().FinalMoveTransform;
        }
    
        /// <summary>
        /// O(1) time. Fast.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static Transform GetFinalLookTransform(this IContext actor)
        {
            return DataRegistry<D_Positioning>.GetData(actor).FinalLookTransform;
        }

        /// <summary>
        /// O(1) time. Fast.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static Transform GetFinalFacingTransform(this IContext actor)
        {
            return DataRegistry<D_Positioning>.GetData(actor).FinalFacingTransform;
        }
    
        /// <summary>
        /// O(1) time. Fast.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static Transform GetFinalMoveTransform(this IContext actor)
        {
            return DataRegistry<D_Positioning>.GetData(actor).FinalMoveTransform;
        }
        public static T GetData<T>(this IContext context,string key = "")
        {
            return DataRegistry<T>.GetData(context,key);
        }
        public static T GetData<T>(this IActor actor,string key = "")
        {
            return DataRegistry<T>.GetData(actor.pContext,key);
        }
        public static T Get<T>(this DataField<T> field,IActor actor)
        {
            return field.Get(actor.pContext);
        }
    
        public static IActor GetActor(this IContext context,string key = "")
        {
            return DataRegistry<IActor>.GetData(context,key);
        }
    
        public static void SetData<T>(this IContext context,T value,string key = "")
        {
            DataRegistry<T>.SetData(context,value,key);
        }
        public static void RemoveData<T>(this IContext context,string key = "")
        {
            DataRegistry<T>.RemoveData(context,key);
        }
        public static bool ContainsData<T>(this IContext context, string key = "")
        {
            return DataRegistry<T>.ContainsData(context,key);
        }
    }
}