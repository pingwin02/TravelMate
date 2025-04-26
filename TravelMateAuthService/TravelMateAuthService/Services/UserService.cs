using TravelMateAuthService.Controllers;
using TravelMateAuthService.Repositories;

namespace TravelMateAuthService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) { 
        _userRepository = userRepository;
        }

        public async Task<Guid?> CheckLoginCredentials(Credentials credentials)
        {
            return await _userRepository.CheckLoginCredentials(credentials);
        }
    }
}
 