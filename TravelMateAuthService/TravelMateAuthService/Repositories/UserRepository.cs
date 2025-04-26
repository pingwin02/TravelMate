using Microsoft.EntityFrameworkCore;
using TravelMateAuthService.Controllers;
using TravelMateAuthService.Data;

namespace TravelMateAuthService.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly DataContext _context;
        public UserRepository(DataContext context) {
            _context = context;
        }

        public async Task<Guid?> CheckLoginCredentials(Credentials credentials)
        {
            var user = await _context.Users
                .Where(u => u.Username == credentials.Username && u.Password == credentials.Password)
                .FirstOrDefaultAsync();
            if (user != null)
                return user.Guid;

            return null;
        }
    }
}
