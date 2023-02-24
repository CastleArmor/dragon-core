public interface IUpdateOwner : ICastable
{
    IConfiguredUpdateBehaviour ConfiguredUpdateHandler { get; set; }
}