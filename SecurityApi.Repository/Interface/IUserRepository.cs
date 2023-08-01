using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApi.Repository.Interface
{
    public interface IUserRepository
    {
        Task<T> ExecuteCommand<T>(string command, DynamicParameters parameters, string connectionStringName) where T : class;
        Task<List<T>> ListUsers<T>(string command, DynamicParameters parameters, string connectionStringName) where T : class;
    }
}
