using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public struct InteractionArgs
{
    //Source with Applier strategy interacts with Target with a Tool. Tool may be null time to time.
    [ShowInInspector]
    public IActor Applier;
    [ShowInInspector]
    public IActor Source;
    [ShowInInspector]
    public IActor Target;
    [ShowInInspector]
    public IActor Tool;
    [ShowInInspector] 
    public PositionalEffectData PositionalEffectData;
}