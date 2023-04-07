namespace Dragon.Core
{
    public interface IEventInstaller
    {
        public void Install(IContext selfMain);
        public void Remove();
    }
}