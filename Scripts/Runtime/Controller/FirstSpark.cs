using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Actor))]
public class FirstSpark : MonoBehaviour
{
    [SerializeField] private Actor _actor;

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (_actor == null)
            {
                _actor = GetComponent<Actor>();
            }
        }
    }
    

    private IEnumerator Start()
    {
        yield return null;
        Application.targetFrameRate = 60;
        _actor.InitializeIfNot();
        _actor.BeginIfNot();
    }
}