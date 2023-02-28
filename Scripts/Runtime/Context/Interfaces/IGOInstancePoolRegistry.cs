using UnityEngine;

public interface IGOInstancePoolRegistry : IData
{
    public void CreatePool(IGOInstance original, int initialCount);
    public IGOInstance Retrieve(GameObject original);
}