using SecurityApi.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace SecurityApi.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {

        public async Task<T> ExecuteCommand<T>(string command, DynamicParameters parameters, string connectionStringName) where T : class
        {
            using(IDbConnection connection = new SqlConnection(connectionStringName))
            {
                var result = await connection.QueryFirstOrDefaultAsync<T>(command, parameters, commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<List<T>> ListUsers<T>(string command, DynamicParameters parameters, string connectionStringName) where T : class
        {
            using (IDbConnection connection = new SqlConnection(connectionStringName))
            {
                var result = await connection.QueryAsync<T>(command, parameters, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }
    }
}
