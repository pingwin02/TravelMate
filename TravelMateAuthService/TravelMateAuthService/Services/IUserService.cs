using TravelMateAuthService.Controllers;

namespace TravelMateAuthService.Services
{
    public interface IUserService
    {
        public Task<bool> CheckLoginCredentials(Credentials credentials);
    }
}
