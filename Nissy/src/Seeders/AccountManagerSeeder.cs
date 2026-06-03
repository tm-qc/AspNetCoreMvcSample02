using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Models.Entity;

namespace Nissy.Seeders
{
    // 初期データ投入クラス(account_manager)
    public class AccountManagerSeeder
    {
        public static async Task Initialize(MyContext db)
        {
            // データが存在したら終了(MyContextのDbSetの名前になる)
            if (await db.AccountManagers.AnyAsync()) return;

            // データ定義(Entityにあるものを定義)
            db.AccountManagers.AddRange(
                new AccountManagerEntity
                {
                    AccountCode = "AC01",
                    CompanyName = "会社名01",
                    ContractorName = "契約者氏名01",
                    //app_userのlogin_codeがvarchar(10)で定義されているため、intのidで暫定でセットにする
                    CreatedId = 1,
                    UpdatedId = 1,
                },
                new AccountManagerEntity
                {
                    AccountCode = "AC02",
                    CompanyName = "会社名02",
                    ContractorName = "契約者氏名02",
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
