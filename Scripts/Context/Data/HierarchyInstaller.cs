using System;
using UnityEngine;

public class HierarchyInstaller : MonoBehaviour
{
    [SerializeField] private DataInstallerGroup _installerGroup;

    private void Awake()
    {
        IDataContext parentContext = GetComponentInParent<IDataContext>();
        _installerGroup.InstallFor(parentContext);
    }
}