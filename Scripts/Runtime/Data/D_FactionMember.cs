using System;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable]
    public class D_FactionMember : ContextData
    {
        [SerializeField] private ActorTagKey _currentFaction;
        public event Action<IContext, ActorTagKey, ActorTagKey> onCurrentFactionChanged;
        public ActorTagKey CurrentFaction
        {
            get => _currentFaction;
            set
            {
                ActorTagKey oldValue = _currentFaction;
                bool isChanged = _currentFaction != value;
                _currentFaction = value;
                if (isChanged)
                {
                    if (oldValue != null)
                    {
                        if (pContext.GetActor().ContainsTag(oldValue))
                        {
                            pContext.GetActor().RemoveTag(oldValue);
                        }                   
                    }

                    if (_currentFaction != null)
                    {
                        pContext.GetActor().AddTag(_currentFaction);
                    }
                    onCurrentFactionChanged?.Invoke(pContext, oldValue, value);
                }
            }
        }
    }
}