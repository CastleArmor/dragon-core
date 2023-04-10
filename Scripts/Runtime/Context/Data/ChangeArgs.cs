namespace Dragon.Core
{
    public struct ChangeArgs<TSender,TValue>
    {
        public TSender Sender;
        public TValue OldValue;
        public TValue NewValue;
    }
}