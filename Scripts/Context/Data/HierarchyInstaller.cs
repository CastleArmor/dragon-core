using System;
using UnityEngine;

public class HierarchyInstaller : MonoBehaviour
{
    [SerializeField] private DataInstallerGroup _installerGroup;

    private void Awake()
    {
        IDataContext parentContext = GetComponentInParent<IDataContext>();
        if (parentContext != null)
        {
            if (!parentContext.IsDataPrepared)
            {
                parentContext.onAllowAdditionalDataOnInitialize += OnMainAllowsDataInstallOnInitialize;
            }
            else
            {
                _installerGroup.InstallFor(parentContext);
            }
        }
        else
        {
            _installerGroup.InstallFor(null);//Global install.
        }
    }
    private void OnMainAllowsDataInstallOnInitialize(IDataContext obj)
    {
        obj.onAllowAdditionalDataOnInitialize -= OnMainAllowsDataInstallOnInitialize;
        _installerGroup.InstallFor(obj);
    }
}