using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable]
    public class DefaultInstallMethod : DataInstallMethod
    {
        [SerializeField] private DataInstallerGroup _installerGroup;

        public override void InstallFor(IDataContext context)
        {
            if (_installerGroup)
            {
                _installerGroup.InstallFor(context);
            }
        }
    }
}