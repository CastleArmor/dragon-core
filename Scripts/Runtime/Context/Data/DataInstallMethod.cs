namespace Dragon.Core
{
    public abstract class DataInstallMethod: IDataInstaller
    {
        public abstract void InstallFor(IContext context);
    }
}