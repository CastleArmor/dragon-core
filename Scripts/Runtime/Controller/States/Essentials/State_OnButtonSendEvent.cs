using UnityEngine;
using UnityEngine.UI;

namespace Dragon.Core
{
    public class State_OnButtonSendEvent : MonoActorState
    {
        [SerializeField] private Button _button;
        [SerializeField] private EventField _event;
        protected override void OnEnter()
        {
            base.OnEnter();
            _button.onClick.AddListener(OnClose);
        }

        private void OnClose()
        {
            _event.Raise(pContext);
        }

        protected override void OnExit()
        {
            base.OnExit();
            _button.onClick.RemoveListener(OnClose);
        }
    }
}