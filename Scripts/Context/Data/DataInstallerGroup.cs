using System.Collections.Generic;
using UnityEngine;

public abstract class DataInstallerGroup : MonoBehaviour, IDataInstaller
{
    public void InstallFor(IDataContext dataContext)
    {
        foreach (IDataInstaller installer in GetInstallers())
        {
            installer.InstallFor(dataContext);
        }
    }

    protected abstract IEnumerable<IDataInstaller> GetInstallers();
}