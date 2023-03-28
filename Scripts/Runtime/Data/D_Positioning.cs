using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable]
    public class D_Positioning : ContextData
    {
        //Moving transform of the object. Probably with rigidbody. This is tank's rails.
        [SerializeField]
        [FoldoutGroup("Settings")] private Transform _moveTransform;
        public Transform MoveTransform 
        {
            get => _moveTransform;
            set
            {
                _moveTransform = value; 
            
                UpdateValues();
            }
        }
    
        //Defines where moving object looks but not rotated towards. This is tank's gun.
        [SerializeField]
        [FoldoutGroup("Settings")] private Transform _lookTransform;
        public Transform LookTransform
        {
            get => _lookTransform;
            set
            {
                _lookTransform = value;
            
                UpdateValues();
            }
        }

        //Defines where object FACES, this is the part for whole rotations. This is tank's body.
        [SerializeField]
        [FoldoutGroup("Settings")] private Transform _facingTransform;
        public Transform FacingTransform
        {
            get => _facingTransform;
            set
            {
                _facingTransform = value;

                UpdateValues();
            }
        }

        [ShowInInspector][ReadOnly] private Transform _finalMoveTransform;
        public Transform FinalMoveTransform 
        {
            get => _finalMoveTransform;
        }
    
        [ShowInInspector][ReadOnly] private Transform _finalLookTransform;
        public Transform FinalLookTransform 
        {
            get => _finalLookTransform;
        }
    
        [ShowInInspector][ReadOnly] private Transform _finalFacingTransform;
        public Transform FinalFacingTransform 
        {
            get => _finalFacingTransform;
        }

        protected override void OnInitializeInstanceData()
        {
            base.OnInitializeInstanceData();
        
            DataContext.onParentContextChanged += OnUserContextChanged;
            if (!_facingTransform)
            {
                _facingTransform = DataContext.GetData<Transform>();
            }
            if (!_lookTransform)
            {
                _lookTransform = DataContext.GetData<Transform>();
            }
            if (!_moveTransform)
            {
                _moveTransform = DataContext.GetData<Transform>();
            }
            UpdateValues();
        }

        protected override void OnRemove()
        {
            base.OnRemove();
            Context.As<IHierarchyContext>().onParentContextChanged -= OnUserContextChanged;
        }

        private void OnUserContextChanged(IContext self, IContext oldUser, IContext newUser)
        {
            UpdateValues();
        }

        private void UpdateValues()
        {
            if (DataContext.ParentContext != null)
            {
                D_Positioning positioning = DataContext.ParentContext.As<IDataContext>().GetData<D_Positioning>();
                _finalFacingTransform = positioning._finalFacingTransform;
                _finalLookTransform = positioning._finalLookTransform;
                _finalMoveTransform = positioning._finalMoveTransform;
            }
            else
            {
                _finalFacingTransform = _facingTransform;
                _finalLookTransform = _lookTransform;
                _finalMoveTransform = _moveTransform;
            }
        }
    }
}