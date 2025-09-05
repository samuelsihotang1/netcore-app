using backend.Models;

namespace backend.Services
{
    public interface ITokenService
    {
        (string accessToken, DateTime expiresAtUtc) CreateAccessToken(AppUser user, IEnumerable<string>? roles = null);
    }
}
