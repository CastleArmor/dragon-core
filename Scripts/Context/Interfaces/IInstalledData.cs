public interface IInstalledData : IData
{
    bool IsInstalled { get; }
    string AssignedID { get; set; }
    void OnInstalledData(IDataContext context);
    void OnRemoveData();
}