public interface IInstalledData : IData
{
    bool IsInstalled { get; }
    string AssignedID { get; set; }
    string KeyID { get; set; }
    void OnInstalledData(IContext context);
    void OnRemoveData();
}