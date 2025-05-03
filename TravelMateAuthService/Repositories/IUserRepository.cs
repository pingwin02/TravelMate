using TravelMateAuthService.Controllers;

namespace TravelMateAuthService.Repositories
{
    public interface IUserRepository
    {
        public Task<Guid?> CheckLoginCredentials(Credentials credentials);
        public Task RegisterUser(Credentials credentials);
    }
}
