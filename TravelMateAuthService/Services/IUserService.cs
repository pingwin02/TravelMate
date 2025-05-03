using TravelMateAuthService.Controllers;

namespace TravelMateAuthService.Services;

public interface IUserService
{
    public Task<Guid?> CheckLoginCredentials(Credentials credentials);
    public Task RegisterUser(Credentials credentials);
}