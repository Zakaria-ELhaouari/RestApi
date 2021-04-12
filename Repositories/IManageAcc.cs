using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Repositories
{
    public interface IManageAcc
    {
        Task<IEnumerable<User>> GetAccs();
        Task<User> GetAcc(string id);
        Task Delete(string id);
    }
}
