using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct AddressField
{
    [SerializeField]
    [HideLabel]
    [HorizontalGroup(GroupID = "install", Width = 0.30f)]
    public DataAddress DataAddress;
    
    [SerializeField]
    [HideLabel]
    [HideIf("HideIfContextAddress")]
    [HorizontalGroup(GroupID = "install", Width = 0.30f)]
    public ContextAddress ContextAddress;

    private bool HideIfContextAddress => DataAddress != DataAddress.Context;
    
    [SerializeField]
    [HideLabel]
    [HideIf("HideIfRelativeAddress")]
    [HorizontalGroup(GroupID = "relative", Width = 0.20f)]
    public RelativeAddress RelativeAddress;
    private bool HideIfRelativeAddress => ContextAddress != ContextAddress.Relative || HideIfContextAddress;
    
    [SerializeField]
    [HideLabel]
    [HideIf("HideIfRelativeAddress")]
    [HorizontalGroup(GroupID = "relative", Width = 0.80f)]
    public RelativeContextStack RelativeStack;
    
    public IContext GetFromAddress(IHierarchyContext context)
    {
        if (DataAddress == DataAddress.Global) return null;
        else
        {
            switch (ContextAddress)
            {
                case ContextAddress.Self: return context;
                case ContextAddress.Parent: return context.ParentContext;
                case ContextAddress.Root: return context.RootContext;
                case ContextAddress.Scene:
                    Scene scene = context.As<IUnityComponent>().gameObject.scene;
                    return ContextRegistry.GetContext(scene.name);
                case ContextAddress.Relative:
                    switch (RelativeAddress)
                    {
                        case RelativeAddress.Self : 
                            return GetRelativeAtAddress(RelativeStack.ContextKeys, context);
                        case RelativeAddress.Parent :
                            return GetRelativeAtAddress(RelativeStack.ContextKeys, context.ParentContext);
                        case RelativeAddress.Root :
                            return GetRelativeAtAddress(RelativeStack.ContextKeys, context.RootContext);
                        case RelativeAddress.Scene :
                            Scene scene2 = context.As<IUnityComponent>().gameObject.scene;
                            return GetRelativeAtAddress(RelativeStack.ContextKeys, ContextRegistry.GetContext(scene2.name));
                    }
                    return context;
            }
            return context;
        }
    }
    
    public IContext GetRelativeAtAddress(List<DataKey> stack,IContext starting)
    {
        if (stack.Count == 0) return null;
        return RecursiveGetRelativeAtAddress(starting,stack, 0);
    }

    /// <summary>
    /// Returns only on initials.
    /// </summary>
    /// <param name="relationOwner"></param>
    /// <param name="stack"></param>
    /// <param name="currentIndex"></param>
    /// <returns></returns>
    private IContext RecursiveGetRelativeAtAddress(IContext relationOwner,List<DataKey> stack, int currentIndex)
    {
        if (stack.Count <= currentIndex)
        {
            return null;
        }
        if (relationOwner.ContainsData<IContext>(stack[currentIndex].ID))
        {
            IContext main = relationOwner.GetData<IContext>(stack[currentIndex].ID);
            IContext nextOwner = main;
            IContext recurse = RecursiveGetRelativeAtAddress(nextOwner,stack, currentIndex + 1);
            if (recurse != null)
            {
                main = recurse;
            }

            return main;
        }
        return null;
    }
}