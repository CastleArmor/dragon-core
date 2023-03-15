using UnityEngine.SceneManagement;

namespace Dragon.Core
{
    public class State_LoadNextScene : MonoActorState
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            //For case its this way.
            SceneManager.LoadScene(gameObject.scene.name);
        }
    }
}