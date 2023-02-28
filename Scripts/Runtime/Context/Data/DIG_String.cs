using System.Collections.Generic;
using UnityEngine;

public class DIG_String : DataInstallerGroup
{
    [SerializeField] private DataInstaller<string> _string;
    protected override IEnumerable<IDataInstaller> GetInstallers()
    {
        yield return _string;
    }
}