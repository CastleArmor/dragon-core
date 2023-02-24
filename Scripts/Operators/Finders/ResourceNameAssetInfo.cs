using Heimdallr.Core;
using UnityEngine;

[System.Serializable]
public struct ResourceNameAssetInfo<T> where T : UnityEngine.Object
{
    public string PathID;
    public T Key;
    public string Path;
}