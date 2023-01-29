using System.Collections.Generic;
using UnityEngine;

public class DIG_ToggleableGroup : DataInstallerGroup
{
    [SerializeField] private DataInstaller<ToggleableGroupDataSet> _toggleableGroup;
    [SerializeField] private DataInstaller<Dictionary<string,IContext>> _groupDictionary;

    protected override IEnumerable<IDataInstaller> GetInstallers()
    {
        yield return _toggleableGroup;
        yield return _groupDictionary;
    }
}