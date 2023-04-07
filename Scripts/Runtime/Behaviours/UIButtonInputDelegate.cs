using UnityEngine;
using UnityEngine.EventSystems;

namespace Dragon.Core
{
    public class UIButtonInputDelegate : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
    {
        [SerializeField] private string _buttonInputName;
        private int _pointerDownFrame;
        private int _pointerUpFrame;

        private void OnEnable()
        {
            HInput.DelegateGetButton(_buttonInputName,OnGetButton);
            HInput.DelegateGetButtonDown(_buttonInputName,OnGetButtonDown);
            HInput.DelegateGetButtonUp(_buttonInputName,OnGetButtonUp);
        }

        private bool OnGetButtonUp(string buttonName)
        {
            return _pointerUpFrame == Time.frameCount;
        }

        private bool OnGetButtonDown(string buttonName)
        {
            return _pointerDownFrame == Time.frameCount;
        }

        private bool OnGetButton(string buttonName)
        {
            return _pointerUpFrame < _pointerDownFrame;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownFrame = Time.frameCount;
            HInput.RaiseButtonDown(_buttonInputName);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pointerUpFrame = Time.frameCount;
            HInput.RaiseButtonUp(_buttonInputName);
        }
    }
}