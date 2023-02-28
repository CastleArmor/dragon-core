using System.Collections.Generic;
using UnityEngine;

public class DIG_PopUpPanel : DataInstallerGroup
{
    [SerializeField] private DataInstaller<float> _someValue;
    [SerializeField] private DataInstaller<Transform> _someOtherValue;

    protected override IEnumerable<IDataInstaller> GetInstallers()
    {
        yield return _someValue;
        yield return _someOtherValue;
    }
}