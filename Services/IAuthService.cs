using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(Register model);
        Task<AuthModel> GetTokenAsync(GetToken model);
    }
}
