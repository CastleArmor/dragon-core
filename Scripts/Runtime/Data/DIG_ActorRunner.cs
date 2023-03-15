using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public class DIG_ActorRunner : DataInstallerGroup
    {
        [SerializeField] private DataInstaller<Dictionary<string, List<IActor>>> _runningDictionary;
        [SerializeField] private DataInstaller<UniqueList<IActor>> _runningList;
        protected override IEnumerable<IDataInstaller> GetInstallers()
        {
            yield return _runningDictionary;
            yield return _runningList;
        }
    }
}