using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TravelMateAuthService.Controllers;
using TravelMateAuthService.Data;
using TravelMateAuthService.Entities;

namespace TravelMateAuthService.Repositories
{
    public class UserRepository(DataContext context) : IUserRepository
    {
        private readonly PasswordHasher<UserCredentials> _passwordHasher = new();

        public async Task<Guid?> CheckLoginCredentials(Credentials credentials)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == credentials.Username);

            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, credentials.Password);

            return result == PasswordVerificationResult.Success ? user.Guid : null;
        }

        public async Task RegisterUser(Credentials credentials)
        {
            var existingUser = await context.Users
                .FirstOrDefaultAsync(u => u.Username == credentials.Username);
            
            if (existingUser != null)
                throw new DuplicateNameException("Username already exists");
            
            var user = new UserCredentials
            {
                Username = credentials.Username,
                Guid = Guid.NewGuid()
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, credentials.Password);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
    }
}