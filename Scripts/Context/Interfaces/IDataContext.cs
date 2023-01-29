using System;

public interface IDataContext : IContext
{
    IDataContext ParentContext { get; set; }
    IDataContext RootContext { get; }
    string DataContextID { get; }
    event Action<IDataContext, IDataContext, IDataContext> onParentContextChanged; 
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