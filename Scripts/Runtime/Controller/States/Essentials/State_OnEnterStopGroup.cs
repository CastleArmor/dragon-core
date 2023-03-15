using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public class State_OnEnterStopGroup : MonoActorState
    {
        [SerializeField] private DataField<List<IActor>> _group;
        [SerializeField] private float _delay;
        protected override void OnEnter()
        {
            base.OnEnter();
            if (_delay > 0)
            {
                StartCoroutine(DelayedRoutine());
                return;
            }
        
            if (_group.TryGet(DataContext))
            {
                foreach (IActor actor in _group.Data)
                {
                    actor.StopIfNot();
                }
            }
        }

        private IEnumerator DelayedRoutine()
        {
            yield return new WaitForSeconds(_delay);
            if (_group.TryGet(DataContext))
            {
                foreach (IActor actor in _group.Data)
                {
                    actor.StopIfNot();
                }
            }
        }
    }
}