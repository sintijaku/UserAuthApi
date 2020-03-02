using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPI.Data;
using UserAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace UserAPI.Services
{
    public class AppUserService : IAppUserService
    {
        private UserContext _context;

        public AppUserService(UserContext context)
        {
            _context = context;
        }

        public async Task<AppUser> Login(string username, string password)
        {
            var appUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.Username == username);

            if (appUser==null)
            {
                return null;
            }
            if (!VerifyPassword(password, appUser.PasswordHash, appUser.PasswordSalt))
            {
                return null;
            }
            return appUser;
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // Create hash using password salt.
                for (int i = 0; i < computedHash.Length; i++)
                { // Loop through the byte array
                    if (computedHash[i] != passwordHash[i]) return false; // if mismatch
                }
            }
            return true; //if no mismatches.
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<AppUser> Register(AppUser appUser, string password)
        {

                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                appUser.PasswordHash = passwordHash;
                appUser.PasswordSalt = passwordSalt;

                await _context.AppUsers.AddAsync(appUser); // Adding the user to context of users.
                await _context.SaveChangesAsync(); // Save changes to database.

                return appUser;
        }

        public async Task<bool> UserExsists(string username)
        {
            if (await _context.AppUsers.AnyAsync(x => x.Username == username))
            {
                return true;
            }
            return false;
        }
    }
}
