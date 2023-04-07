using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public abstract class DataInstallerGroup : MonoBehaviour, IDataInstaller
    {
        public void InstallFor(IContext dataContext)
        {
            foreach (IDataInstaller installer in GetInstallers())
            {
                installer.InstallFor(dataContext);
            }
        }

        protected abstract IEnumerable<IDataInstaller> GetInstallers();
    }
}