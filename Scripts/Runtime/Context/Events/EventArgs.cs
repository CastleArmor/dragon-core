namespace Dragon.Core
{
    [System.Serializable]
    public struct EventArgs
    {
        public IContext EventContext;
        public string EventName;
    }
}