using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dragon.Core
{
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
        [HorizontalGroup(GroupID = "install", Width = 0.30f)][GUIColor("GetContextAddressColor")]
        public ContextAddress ContextAddress;

        private Color GetContextAddressColor()
        {
            Color returned = Color.white;
            if (ContextAddress == ContextAddress.Parent)
            {
                returned = Color.cyan;
            }
            else if (ContextAddress == ContextAddress.Relative)
            {
                returned = new Color(0.8f,0.5f,0.8f);
            }
            else if (ContextAddress == ContextAddress.Root)
            {
                returned = new Color(0.8f,0.8f,0.5f);
            }
            else if (ContextAddress == ContextAddress.Scene)
            {
                returned = new Color(1f,0.4f,0.4f);
            }

            return returned;
        }

        private bool HideIfContextAddress => DataAddress != DataAddress.Context;
    
        [SerializeField]
        [HideLabel]
        [HideIf("HideIfRelativeAddress")]
        [HorizontalGroup(GroupID = "relative", Width = 0.20f)]
        public RelativeAddress RelativeAddress;
        private bool HideIfRelativeAddress => ContextAddress != ContextAddress.Relative || HideIfContextAddress;
    
        [SerializeField]
        [HideLabel]
        [HideIf("HideIfGroupKey")]
        public DataKey GroupKey;
        private bool HideIfGroupKey => DataAddress != DataAddress.GroupFirstMember;
    
        [SerializeField]
        [HideLabel]
        [HideIf("HideIfRelativeAddress")]
        [HorizontalGroup(GroupID = "relative", Width = 0.80f)]
        public RelativeContextStack RelativeStack;
    
        public static AddressField SingleContextRoot()
        {
            return new AddressField()
            {
                ContextAddress = ContextAddress.Root,
                DataAddress = DataAddress.Context
            };
        }
    
        public IContext GetFromAddress(IContext context)
        {
            if (DataAddress == DataAddress.Global) return null;
            else
            {
                if (DataAddress == DataAddress.GroupFirstMember)
                {
                    return DataRegistry<List<IActor>>.GetData(null, GroupKey.ID)[0].pContext;
                }
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
}