using UnityEngine;

namespace Dragon.Core
{
    public class ActorPointer : MonoBehaviour
    {
        [SerializeField] private Actor _pointed;
        public Actor Pointed => _pointed;
    }
}