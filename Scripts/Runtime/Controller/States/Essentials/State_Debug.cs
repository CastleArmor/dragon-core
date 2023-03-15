using UnityEngine;

namespace Dragon.Core
{
    public class State_Debug : MonoActorState
    {
        [SerializeField] private string _debug;
        protected override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Entered : " + _debug);
        }

        protected override void OnExit()
        {
            base.OnExit();
            Debug.Log("Exited : " + _debug);
        }
    }
}