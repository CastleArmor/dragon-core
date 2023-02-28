using UnityEngine;
using UnityEngine.UI;

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
        _event.Raise(EventContext);
    }

    protected override void OnExit()
    {
        base.OnExit();
        _button.onClick.RemoveListener(OnClose);
    }
}