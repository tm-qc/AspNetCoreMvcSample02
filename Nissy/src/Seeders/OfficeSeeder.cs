using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Models.Entity;

namespace Nissy.Seeders
{
    // 初期データ投入クラス(office)
    public class OfficeSeeder
    {
        public static async Task Initialize(MyContext db)
        {
            // データが存在したら終了(MyContextのDbSetの名前になる)
            if (await db.Offices.AnyAsync()) return;

            // 親のIdはDBから取得する
            var accountManagers = await db.AccountManagers.ToListAsync();

            // 親の数だけOfficeのデータを作成
            var data = accountManagers.Select(item => new OfficeEntity
            {
                AccountManagerId = item.Id,
                CompanyName = $"{item.CompanyName}_Office",
                CreatedId = item.CreatedId,
                UpdatedId = item.UpdatedId,
            }).ToList();

            db.Offices.AddRange(data);
            await db.SaveChangesAsync();
        }

    }
}
