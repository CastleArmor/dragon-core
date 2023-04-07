using UnityEngine;

namespace Dragon.Core
{
    public class UnityInputDelegate : MonoBehaviour
    {
        [SerializeField] private string _axisInput;
        [SerializeField] private string _buttonInput;
        [SerializeField] private bool _raiseEvents;

        private void OnEnable()
        {
            HInput.DelegateGetAxis(_axisInput,OnGetAxis);
            HInput.DelegateGetButton(_buttonInput,OnGetButton);
            HInput.DelegateGetButtonUp(_buttonInput,OnGetButtonUp);
            HInput.DelegateGetButtonDown(_buttonInput,OnGetButtonDown);
        }

        private void Update()
        {
            if (!_raiseEvents)
            {
                return;
            }

            if (Input.GetButtonDown(_buttonInput))
            {
                HInput.RaiseButtonDown(_buttonInput);
            }

            if (Input.GetButtonUp(_buttonInput))
            {
                HInput.RaiseButtonUp(_buttonInput);
            }
        }

        private bool OnGetButtonUp(string buttonName)
        {
            return Input.GetButtonUp(buttonName);
        }

        private bool OnGetButtonDown(string buttonName)
        {
            return Input.GetButtonDown(buttonName);
        }

        private bool OnGetButton(string buttonName)
        {
            return Input.GetButton(buttonName);
        }

        private float OnGetAxis(string inputName)
        {
            return Input.GetAxis(inputName);
        }
    }
}