namespace HWPLoginControl.Data
{
    public interface IDataAccess
    {
        Task<IEnumerable<T>> LoadData<T, U>(string query, U param, string connectionsid = "Default");
        Task<int> SaveData<U>(string query, U param, string connectionsid = "Default");
    }
}