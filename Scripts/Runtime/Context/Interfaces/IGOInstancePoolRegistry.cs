using UnityEngine;

namespace Dragon.Core
{
    public interface IGOInstancePoolRegistry : IContextData
    {
        public void CreatePool(IGOInstance original, int initialCount);
        public IGOInstance Retrieve(GameObject original);
    }
}