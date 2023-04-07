namespace Dragon.Core
{
    public static class GIPool<T> where T : new()
    {
        private static InstancePool<T> _instancePool = new InstancePool<T>();
        
        public static void Create(int count)
        {
            _instancePool.Create(count);
        }

        public static void Clear()
        {
            _instancePool.Clear();
        }

        public static T Get()
        {
            return _instancePool.Get();
        }

        public static void Return(T ins)
        {
            _instancePool.Return(ins);
        }
    }
}