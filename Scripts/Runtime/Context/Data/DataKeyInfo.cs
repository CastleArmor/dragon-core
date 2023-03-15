namespace Dragon.Core
{
    [System.Serializable]
    public struct DataKeyInfo
    {
        public string PathID;
        public DataKey Key;
        public string Path;
    }
    [System.Serializable]
    public struct EventKeyInfo
    {
        public string PathID;
        public EventKey Key;
        public string Path;
    }
}