using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Models.Entity;

namespace Nissy.Seeders
{
    // 初期データ投入クラス(app_user)
    public class AppUserSeeder
    {
        public static async Task Initialize(MyContext db)
        {
            // データが存在したら終了(MyContextのDbSetの名前になる)
            if (await db.AppUsers.AnyAsync()) return;

            // 親のIdはDBから取得する
            var accountManagers = await db.AccountManagers.ToListAsync();

            var hasher = new PasswordHasher<AppUserEntity>();

            // 親の数だけデータを作成
            var data = accountManagers.Select((item, index) =>
            {
                var loginCode = $"AU{(index + 1):D2}";//AU01、AU02、AU03...
                var plainPassword = $"{loginCode}p";//AU01p、AU02p、AU03p...

                var user = new AppUserEntity
                {
                    AccountManagerId = item.Id,
                    LoginCode = loginCode,
                    Name = $"Name {(index + 1):D2}",//D2：2桁埋め(01)
                    NameKana = $"ネーム {(index + 1):D2}",//D2：2桁埋め(01)
                    DisplayOrder = index + 1,
                    CreatedId = item.CreatedId,
                    UpdatedId = item.UpdatedId,
                };

                // パスワードをハッシュ化して保存
                user.PasswordHash = hasher.HashPassword(user, plainPassword);
                return user;

            }).ToList();

            db.AppUsers.AddRange(data);
            await db.SaveChangesAsync();
        }

    }
}
