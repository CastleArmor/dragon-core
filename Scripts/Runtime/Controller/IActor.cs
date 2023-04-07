using System;

namespace Dragon.Core
{
    public interface IActor : IUnityComponent,IInitializable,IRunnable
    {
        IGOInstancePoolRegistry GOPool { get; }
        bool IsBeingDestroyed { get; }
        bool IsEnded { get; }
        float TimeScale { get; set; }
        string EndingEventID { get; }
        string ObjectTypeID { get; }
        IContext pContext { get; }
        event Action<IActor, float, float> onTimeScaleChanged;
    
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
}