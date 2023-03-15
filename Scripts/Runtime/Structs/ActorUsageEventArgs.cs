[System.Serializable]
public struct ActorUsageEventArgs
{
    public string UsageRequestID;
    public IGOInstance PrefabOrInstance;
    public IActor ActorInstance;
}