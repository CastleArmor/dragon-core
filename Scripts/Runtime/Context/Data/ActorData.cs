namespace Dragon.Core
{
    [System.Serializable]
    public class ActorData : ContextData
    {
        private IActor _actor;
        public IActor Actor => _actor;

        protected override void OnAssignedDataContext()
        {
            base.OnAssignedDataContext();
            _actor = DataContext.GetActor();
        }
    }
}