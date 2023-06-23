using Microsoft.AspNetCore.Identity;

namespace News.Core.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(IdentityUser appUser, UserManager<IdentityUser> userManager);
    }
}
