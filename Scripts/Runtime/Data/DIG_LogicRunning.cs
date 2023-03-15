using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public class DIG_LogicRunning : DataInstallerGroup
    {
        [SerializeField] private DataInstaller<D_LogicRunning> _logicRunning;

        protected override IEnumerable<IDataInstaller> GetInstallers()
        {
            yield return _logicRunning;
        }
    }
}