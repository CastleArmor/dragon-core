using System;

namespace Dragon.Core
{
    public interface IHierarchyContext : IContext
    {
        IContext ParentContext { get; set; }
        IContext RootContext { get; }
        event Action<IContext, IContext, IContext> onParentContextChanged; 
    }

    public interface IDataContext : IHierarchyContext, IUnityObject
    {
        event Action<IDataContext> onInitializeData;
        event Action<IDataContext> onAllowAdditionalDataOnInitialize;
        event Action<IDataContext> onRequestSaveData; 
        event Action<IDataContext> onRequestLoadData; 
        bool IsDefaultPrefabInstance { get; }
        bool IsDataPrepared { get; }
        bool IsPrefab { get; }
        T GetData<T>();
        T GetData<T>(string key);
        void SetData<T>(T value);
        void SetData<T>(string key, T value);
        event Action<IDataContext> onDestroyDataContext;
    }
}