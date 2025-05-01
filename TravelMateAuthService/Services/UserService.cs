using TravelMateAuthService.Controllers;
using TravelMateAuthService.Repositories;

namespace TravelMateAuthService.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<Guid?> CheckLoginCredentials(Credentials credentials)
        {
            return await userRepository.CheckLoginCredentials(credentials);
        }

        public async Task RegisterUser(Credentials credentials)
        {
            await userRepository.RegisterUser(credentials);
        }
    }
}