using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable]
    public class D_GOInstancePoolRegistry : InstalledData,IGOInstancePoolRegistry
    {
        protected override void OnBindAdditional(IContext context)
        {
            DataRegistry<IGOInstancePoolRegistry>.BindData(context,this,KeyID);
        }

        [ShowInInspector][ReadOnly]
        private Dictionary<IGOInstance, GOInstancePool> _pools = new Dictionary<IGOInstance, GOInstancePool>();

        public void CreatePool(IGOInstance original, int initialCount)
        {
            _pools[original] = new GOInstancePool();
            GameObject parent = new GameObject(original.name + "-Pool");
            if (original.TryGetComponent(out RectTransform rect))
            {
                parent.AddComponent<RectTransform>();
            }
            _pools[original].Initialize(original,initialCount,parent.transform);
        }

        public IGOInstance Retrieve(GameObject original)
        {
            if (!_pools.ContainsKey(original.GetComponent<IGOInstance>()))
            {
                CreatePool(original.GetComponent<IGOInstance>(),1);
            }
            return _pools[original.GetComponent<IGOInstance>()].Retrieve();
        }
    }
}