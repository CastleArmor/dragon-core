using UnityEngine;

public class ActorPointer : MonoBehaviour
{
    [SerializeField] private Actor _pointed;
    public Actor Pointed => _pointed;
}