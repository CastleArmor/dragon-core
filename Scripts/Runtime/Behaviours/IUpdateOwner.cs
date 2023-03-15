namespace Dragon.Core
{
    public interface IUpdateOwner : ICastable
    {
        IConfiguredUpdateBehaviour ConfiguredUpdateHandler { get; set; }
    }
}