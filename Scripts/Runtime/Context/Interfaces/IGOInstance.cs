namespace Dragon.Core
{
    public interface IGOInstance : IUnityComponent
    {
        string ObjectTypeID { get; }
        bool IsDefaultPrefabInstance { get; set; }
        bool IsBeingDestroyed { get; }
        bool IsPooledInstance { get; }
        bool IsRetrievedFromPool { get; }
        void PoolCheckoutRegisteredToPool(IGOInstancePool pool);
        void PoolCheckoutRetrievedFromPool();
        void PoolCheckoutReturnedToPool();
        void ReturnPool();
    }
}