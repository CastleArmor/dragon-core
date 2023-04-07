namespace Dragon.Core
{
    public struct DataOnChangeArgs<T>
    {
        public string AssignedKey;
        public IContext Context;
        public T OldValue;
        public T NewValue;
    }
}