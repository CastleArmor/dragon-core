using System;

namespace Dragon.Core
{
    public interface IContext : ICastable
    {
        string name { get; set; }
        string ContextID { get; }
        void InitializeIfNot();
        void InstallDataIfNot();
        event Action<IContext> onDestroyContext;
        
        event Action<IContext> onInitializeData;
        event Action<IContext> onAllowAdditionalDataOnInitialize;
        event Action<IContext> onRequestSaveData; 
        event Action<IContext> onRequestLoadData; 
        bool IsDefaultPrefabInstance { get; }
        bool IsDataPrepared { get; }
        bool IsPrefab { get; }
        T GetData<T>();
        T GetData<T>(string key);
        void SetData<T>(T value);
        void SetData<T>(string key, T value);
        
        IContext ParentContext { get; set; }
        IContext RootContext { get; }
        event Action<IContext, IContext, IContext> onParentContextChanged;
    }
}