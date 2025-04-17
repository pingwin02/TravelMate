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

        public async Task<bool> CheckLoginCredentials(Credentials credentials)
        {
            if(await _context.Users.FirstOrDefaultAsync(x=>x.Username == credentials.Username && x.Password == credentials.Password) != null)
                return true;

            return false;
        }
    }
}
