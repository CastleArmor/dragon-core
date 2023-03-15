using System.Collections.Generic;

namespace Dragon.Core
{
    public class DIG_MultipleOnGameObject : DataInstallerGroup
    {
        protected override IEnumerable<IDataInstaller> GetInstallers()
        {
            List<IDataInstaller> installers = new List<IDataInstaller>(GetComponents<IDataInstaller>());
            installers.Remove(this);
            return installers;
        }
    }
}