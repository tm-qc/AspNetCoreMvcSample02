using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Models.Entity;

namespace Nissy.Seeders
{
    // 初期データ投入クラス(account_manager)
    public class ServiceTypeSeeder
    {
        public static async Task Initialize(MyContext db)
        {
            // データが存在したら終了(MyContextのDbSetの名前になる)
            if (await db.ServiceTypes.AnyAsync()) return;

            // データ定義(Entityにあるものを定義)
            db.ServiceTypes.AddRange(
                new ServiceTypeEntity
                {
                    ServiceDivision = "01",
                    //app_userのlogin_codeがvarchar(10)で定義されているため、intのidで暫定でセットにする
                    CreatedId = 1,
                    UpdatedId = 1,
                },
                new ServiceTypeEntity
                {
                    ServiceDivision = "02",
                    //app_userのlogin_codeがvarchar(10)で定義されているため、intのidで暫定でセットにする
                    CreatedId = 1,
                    UpdatedId = 1,
                },
                new ServiceTypeEntity
                {
                    ServiceDivision = "03",
                    //app_userのlogin_codeがvarchar(10)で定義されているため、intのidで暫定でセットにする
                    CreatedId = 1,
                    UpdatedId = 1,
                }

            );

            // 親データ追加
            await db.SaveChangesAsync();
        }

    }
}
