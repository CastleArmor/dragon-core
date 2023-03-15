namespace Dragon.Core
{
    public interface IData : ICastable
    {
        IDataContext DataContext { get; }
    }
}