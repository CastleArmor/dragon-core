using UnityEngine;

namespace Dragon.Core
{
    public class HierarchyInstaller : MonoBehaviour
    {
        [SerializeField] private DataInstallerGroup _installerGroup;

        private void Awake()
        {
            IContext parentContext = GetComponentInParent<IContext>();
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
        private void OnMainAllowsDataInstallOnInitialize(IContext obj)
        {
            obj.onAllowAdditionalDataOnInitialize -= OnMainAllowsDataInstallOnInitialize;
            _installerGroup.InstallFor(obj);
        }
    }
}