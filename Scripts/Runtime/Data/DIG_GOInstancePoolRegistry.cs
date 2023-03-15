using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public class DIG_GOInstancePoolRegistry : DataInstallerGroup
    {
        [SerializeField] private DataInstaller<D_GOInstancePoolRegistry> _poolData;
        protected override IEnumerable<IDataInstaller> GetInstallers()
        {
            yield return _poolData;
        }
    }
}