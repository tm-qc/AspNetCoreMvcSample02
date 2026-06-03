using Microsoft.EntityFrameworkCore;
using Nissy.Models.Entity;

namespace Nissy.Models
{
    //最低限のコンテキストクラス
    //dbcontext-efというスニペットでひな形作成つくれるが、VS CDOE限定なので、VS2022では使えないっぽい
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        // =============================================
        // Model配下のEntityファイルを追加したら必ずここに追加する。
        // マイグレーションファイルが正しく作成されるために必要。
        // =============================================
        // DbSetの命名規則：<Entityのクラス名> Entityのクラス名の複数形(PascalCaseで英語的に正しいものにする）
        public DbSet<AccountManagerEntity> AccountManagers { get; set; }
        public DbSet<OfficeEntity> Offices { get; set; }
        public DbSet<AppUserEntity> AppUsers { get; set; }
        public DbSet<ServiceTypeEntity> ServiceTypes { get; set; }
        public DbSet<DriverEntity> Drivers { get; set; }


        // テーブル定義の設定(Configurationsフォルダ)を読み込むためのメソッド
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // テーブル定義の設定はファイルごとに分割して作成し以下で一括で読み込む
            // ※テーブル定義の設定はこのフォルダ「Models\Configurations\」
            // アセンブリ内のIEntityTypeConfiguration実装を全部自動で読み込む
            // フォルダ指定とかいらない理由：IEntityTypeConfiguration<T> を実装しているクラスを自動で発見・適用するため
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyContext).Assembly);

            // ソフトデリートのクエリフィルタの例
            // is_deletedカラム追加し、以下の設定をすれば全クエリに自動で IsDeleted == false が適用され、ソフトデリートされたものは取得されなくなる
            // ※削除済みのものも取得したい場合は、クエリ実行時に _context.Drivers.IgnoreQueryFilters() を指定すれば取得できる
            //modelBuilder.Entity<Driver>().HasQueryFilter(d => d.IsDeleted == false);
        }
    }
}
