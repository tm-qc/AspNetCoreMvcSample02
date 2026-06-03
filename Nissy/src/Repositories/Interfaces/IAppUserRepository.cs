using Nissy.Models.Entity;

public interface IAppUserRepository
{
    Task<AppUserEntity?> GetByLoginCodeAsync(string loginCode);

    Task UpdateAsync(AppUserEntity user);
}