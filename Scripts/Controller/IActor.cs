using System;

public interface IActor : IUnityComponent,IInitializable,IRunnable
{
    IGOInstancePoolRegistry GOPool { get; }
    bool IsBeingDestroyed { get; }
    bool IsEnded { get; }
    string EndingEventID { get; }
    IDataContext DataContext { get; }
    IEventContext EventContext { get; }
    
    //Methods
    void CheckoutFinished(string eventID);
    void CheckoutCancelled(string eventID);
    void Cancel(string requestID);
    
    //Events
    event Action<IActor> onInitialize; 
    event Action<IActor> onBeginBeforeLogic;
    event Action<IActor> onBegin;
    event Action<IActor> onStop;     
    event Action<IActor> onFinishEnded;
    event Action<IActor> onCancelEnded;
    event Action<IActor> onEnded;
    event Action<IActor> onEndedStateChanged;
    event Action<IActor,string> onRequestCancel;
}