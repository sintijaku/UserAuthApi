using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPI.Models;

namespace UserAPI.Services
{
    public interface IAppUserService
    {
        Task<AppUser> Register(AppUser appUser, string password);
        Task<AppUser> Login(string username, string password);
        Task<bool> UserExsists(string username);
    }
}
