using System.Collections.Generic;
using UnityEngine;

public class DIG_Global : DataInstallerGroup
{
    [SerializeField] private DataInstaller<D_LogicRunning> _gameModeData;
    [SerializeField] private DataInstaller<D_GOInstancePoolRegistry> _poolData;

    protected override IEnumerable<IDataInstaller> GetInstallers()
    {
        yield return _gameModeData;
        yield return _poolData;
    }
}