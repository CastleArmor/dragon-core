using System;

public interface IActor : IUnityComponent,IInitializable,IRunnable
{
    IGOInstancePoolRegistry GOPool { get; }
    bool IsBeingDestroyed { get; }
    bool IsEnded { get; }
    string EndingEventID { get; }
    string ObjectTypeID { get; }
    IDataContext DataContext { get; }
    IEventContext EventContext { get; }
    
    //Methods
    void FinishIfNotEnded(string eventID);
    void CancelIfNotEnded(string eventID);
    
    //Events
    event Action<IActor> onInitialize; 
    event Action<IActor> onBeginBeforeLogic;
    event Action<IActor> onBegin;
    event Action<IActor> onStop;
    event Action<IActor> onDestroyActor; 
    event Action<IActor> onFinishEnded;
    event Action<IActor> onCancelEnded;
    event Action<IActor> onEnded;
    event Action<IActor> onEndedStateChanged;
}