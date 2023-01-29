using UnityEngine.SceneManagement;

public class State_LoadNextScene : MonoActorState
{
    protected override void OnEnter()
    {
        base.OnEnter();
        //For case its this way.
        SceneManager.LoadScene(gameObject.scene.name);
    }
}