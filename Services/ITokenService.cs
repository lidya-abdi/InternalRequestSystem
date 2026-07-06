using InternalRequestSystem.Models;

namespace InternalRequestSystem.Services
{
    public interface ITokenService
    {
        string GenerateToken(AppUser user);
    }
}
