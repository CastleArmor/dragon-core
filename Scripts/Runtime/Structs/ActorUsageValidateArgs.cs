namespace Dragon.Core
{
    [System.Serializable]
    public struct ActorUsageValidateArgs
    {
        public string UsageRequestID;
        public IGOInstance PrefabOrInstance;
        public DelegatedObject<bool> DelegateObject;
    }
}