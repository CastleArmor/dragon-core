namespace Dragon.Core
{
    public interface IEventInstaller
    {
        public void Install(IEventContext selfMain);
        public void Remove();
    }
}