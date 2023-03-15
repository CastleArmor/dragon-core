using System;
using System.Collections;
using UnityEngine;

namespace Dragon.Core
{
    public class StaticUpdate : GlobalSingleton<StaticUpdate>
    {
        public static event Action<bool> onApplicationPause
        {
            add
            {
                Ensure();
                _onApplicationPause += value;
            }
            remove => _onApplicationPause -= value;
        }
        public static event Action onApplicationQuit
        {
            add
            {
                Ensure();
                _onApplicationQuit += value;
            }
            remove => _onApplicationQuit -= value;
        }
        public static event Action onUpdate
        {
            add
            {
                Ensure();
                _onUpdate += value;
            }
            remove => _onUpdate -= value;
        }
        public static event Action onFixedUpdate
        {
            add
            {
                Ensure();
                _onFixedUpdate += value;
            }
            remove => _onFixedUpdate -= value;
        }
        public static event Action onLateUpdate
        {
            add
            {
                Ensure();
                _onLateUpdate += value; 
            }
            remove => _onLateUpdate -= value;
        }

        private static event Action<bool> _onApplicationPause; 
        private static event Action _onApplicationQuit;
        private static event Action _onUpdate;
        private static event Action _onFixedUpdate;
        private static event Action _onLateUpdate;

        public static Coroutine StartCoroutineStatic(IEnumerator enumerator)
        {
            return Instance.StartCoroutine(enumerator);
        }
        private void Update()
        {
            _onUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            _onFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            _onLateUpdate?.Invoke();
        }

        private void OnApplicationQuit()
        {
            _onApplicationQuit?.Invoke();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _onApplicationPause?.Invoke(pauseStatus);
        }
    }
}