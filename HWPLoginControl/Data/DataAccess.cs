using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace HWPLoginControl.Data
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration _config;

        public DataAccess(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<T>> LoadData<T, U>(string query, U param, string connectionsid = "Default")
        {
            using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionsid));

            return await connection.QueryAsync<T>(query, param);
        }

        public async Task<int> SaveData<U>(string query, U param, string connectionsid = "Default")
        {
            using IDbConnection connection = new MySqlConnection(_config.GetConnectionString(connectionsid));

            return await connection.ExecuteAsync(query, param);
        }
    }
}
