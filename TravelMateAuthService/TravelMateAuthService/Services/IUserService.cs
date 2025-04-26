using TravelMateAuthService.Controllers;

namespace TravelMateAuthService.Services
{
    public interface IUserService
    {
        public Task<Guid?> CheckLoginCredentials(Credentials credentials);
    }
}
