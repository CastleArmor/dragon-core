public interface ISaveableData : ICastable
{
    bool IsPersistent { get; set; }
    void SaveData();
}