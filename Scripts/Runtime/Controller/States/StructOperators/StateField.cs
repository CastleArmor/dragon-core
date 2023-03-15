using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dragon.Core
{
    public enum StateType
    {
        MonoState,
        AssetState
    }

    [TopTitle(
        NameSuffix = "<color=#ffa50055><b>⌥STATE</b></color>",
        NamePrefix = "<color=#ffa50055><b>⌥</b></color>",
        BoldName = true,SetTransform = true,SetName = true,ShowNameOnPrefix = true,HideNameOnMid = true)]
    [System.Serializable][GUIColor(1f,0.9f,0.65f)]
    public struct StateField
    {
        [SerializeField][HideInInspector]
        private string _name;
        public string name
        {
            get => _name;
            set => _name = value;
        }
        
        [SerializeField][HideInInspector] private Transform _transform;
        public Transform transform
        {
            get => _transform;
            set => _transform = value;
        }
    
        [SerializeField][HideLabel][HorizontalGroup] private StateType _stateType;

        public void InitializeIfNeedsInitialize()
        {
            if (State is IInitializedSubState initState)
            {
                if(!initState.IsInitialized) initState.Initialize();
            }
        }
        public bool UseMono => _stateType == StateType.MonoState;
    
        [ShowIf("UseMono")][HorizontalGroup("Ref")][GUIColor(1f,1f,1f)][HideLabel][InlineButton("CreateMonoState","Create")]
        public MonoActorState MonoState;

        public bool TryCheckoutEnter(IActor actor)
        {
            if (State == null) return false;
            State.CheckoutEnter(actor);
            return true;
        }

        public bool TryCheckoutExit(IActor actor)
        {
            if (State == null) return false;
            State.CheckoutExit();
            return true;
        }

        private void CreateMonoState()
        {
#if UNITY_EDITOR
            GameObject gameObject = new GameObject(ObjectNames.NicifyVariableName(name));
            gameObject.transform.SetParent(transform);
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
            GOMultiState multiState = gameObject.AddComponent<GOMultiState>();
            MonoState = multiState;
#endif
        }
    
        public IActorState State
        {
            get
            {
                if (_stateType == StateType.MonoState)
                {
                    return MonoState;
                }

                return null;
            }
        }
    }
}