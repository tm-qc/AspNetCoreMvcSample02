using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Models.Entity;
using Nissy.Repositories.Interfaces;

namespace Nissy.Repositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly MyContext _context;

    public AppUserRepository(MyContext context)
    {
        _context = context;
    }

    /// <summary>
    /// login_code(ID)でユーザー検索
    /// </summary>
    /// <param name="loginCode"></param>
    /// <returns></returns>
    public async Task<AppUserEntity?> GetByLoginCodeAsync(string loginCode)
    {
        return await _context.AppUsers
            .FirstOrDefaultAsync(x => x.LoginCode == loginCode);
    }

    /// <summary>
    /// ユーザデータを更新
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task UpdateAsync(AppUserEntity user)
    {
        _context.AppUsers.Update(user);
        await _context.SaveChangesAsync();
    }
}