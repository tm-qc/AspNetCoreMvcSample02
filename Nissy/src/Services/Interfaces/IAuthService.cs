using Nissy.Models.Entity;

namespace Nissy.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AppUserEntity?> AuthenticateAsync(string loginCode, string password);
    }
}
