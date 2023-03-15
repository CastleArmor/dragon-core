using System;
using Sirenix.OdinInspector;

namespace Dragon.Core
{
    [System.Serializable]
    public class UpdateListener
    {
        public Action onInvoked;
        [ShowInInspector][ReadOnly]
        private bool _wasRegistered;

        public bool IsRegistered => _wasRegistered;
        private bool _registeredWithUpdateHandler;
        private string _registeredUpdateID;
    
        public void RegisterTry()
        {
            if (_wasRegistered) return;
            StaticUpdate.onUpdate += OnInvoke;
            _wasRegistered = true;
            _registeredWithUpdateHandler = false;
        }

        public void UnregisterTry()
        {
            if (!_wasRegistered) return;
            StaticUpdate.onUpdate -= OnInvoke;
            _wasRegistered = false;
        }

        private void OnInvoke()
        {
            if (!_wasRegistered) return; 
            onInvoked?.Invoke();
        }
    }
}