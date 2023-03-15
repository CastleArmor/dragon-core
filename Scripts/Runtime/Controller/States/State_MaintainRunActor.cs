namespace Dragon.Core
{
    public class State_MaintainRunActor : State_RunActor
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            RunActor();
        }
    }
}