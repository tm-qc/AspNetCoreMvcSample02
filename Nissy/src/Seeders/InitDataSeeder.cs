using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Models.Entity;

namespace Nissy.Seeders
{
    // データの初期投入クラス
    // 起動時にデータがなければ投入する
    public class InitDataSeeder
    {
        public static async Task Initialize(IServiceProvider provider)
        {
            //MyContextを呼び出すために、providerからMyContextのインスタンスを作成
            using MyContext db = new MyContext(provider.GetRequiredService<DbContextOptions<MyContext>>());

            // appsettings.json系のファイルを読み込むための機能を取得
            var config = provider.GetRequiredService<IConfiguration>();
            // 設定ファイルからリセットフラグ "Seeder:Reset"を取得
            var isReset = config.GetValue<bool>("Seeder:Reset");


            // =============================================
            // Seederの管理
            // テーブル追加の時に起動時にサンプルデータ投入したいときは削除のSQL + Seederクラスを追加する
            // =============================================

            // リセットするときだけ削除する
            if (isReset)
            {
                try
                {
                    // 同じ接続を取得して全部そこで実行する(FK制御維持のために必要)
                    await db.Database.OpenConnectionAsync();

                    // FK制約を一時的に無効化（TRUNCATEはFKがあるとエラーになるため）
                    await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");

                    // 子→親の順番FKの順番を気にせず全部TRUNCATEできる（AUTO_INCREMENTも自動リセット）
                    await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE driver;");
                    await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE app_user;");
                    await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE office;");
                    await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE service_type;");
                    await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE account_manager;");
                }
                catch (Exception ex)
                {
                    // ログにだすなどの処理
                    Console.WriteLine("リセット処理失敗: " + ex.Message);
                }
                finally
                {
                    // 成功でもエラーでも必ずすること
                    await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");// FK制約を有効化
                    await db.Database.CloseConnectionAsync(); // 接続を閉じる
                }
            }

            // 親→子の順番で呼び出して初期データ投入する
            // ※データがある場合は投入されない
            await AccountManagerSeeder.Initialize(db);
            await ServiceTypeSeeder.Initialize(db);
            await OfficeSeeder.Initialize(db);
            await AppUserSeeder.Initialize(db);
        }

    }
}
