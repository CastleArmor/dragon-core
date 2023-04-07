namespace Dragon.Core
{
    public interface IContextInstallable
    {
        IContext pContext { get; }
        bool IsInstalled { get; }
        void SetInstallParameters(IContext context, string key, string assignedID);
    }
}