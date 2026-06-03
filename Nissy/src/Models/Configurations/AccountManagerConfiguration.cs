using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nissy.Models.Entity;

namespace Nissy.Models.Configurations
{
    // テーブル定義を記載（Fluent APIでの記載）
    /* 設定方法メモ
     * 
     * not null+必須：.IsRequired()のみ
     * not null+任意：.IsRequired()+.HasDefaultValue(初期値)
     * 型がDateTimeの場合：どちらかで適用(MySQLのdatetime型の場合)
     *          .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")//マイクロ秒まで必要な場合は(6)つける
     *          .HasColumnType("datetime")//秒までで良い場合は追加
     */
    public class AccountManagerConfiguration : IEntityTypeConfiguration<AccountManagerEntity>
    {
        //TODO:あくまでサンプル。適宜調整
        public void Configure(EntityTypeBuilder<AccountManagerEntity> entity)
        {
            entity.ToTable("account_manager", t => t.HasComment("アカウント管理マスタ"));

            entity.HasKey(e => e.Id);//主キー設定
            entity.Property(e => e.Id)
                .HasColumnName("id")//カラム名指定(デフォはProperty名でPascalCaseになるので指定する)
                .ValueGeneratedOnAdd()//オートインクリメント
                .HasComment("ID");

            //not null、必須
            entity.Property(e => e.AccountCode)
                .HasColumnName("account_code")
                .IsRequired()//not null
                .HasMaxLength(10)
                .HasComment("アカウントコード");

            //not null、必須
            entity.Property(e => e.CompanyName)
                .HasColumnName("company_name")
                .IsRequired()//not null
                .HasMaxLength(50)
                .HasComment("会社名");

            //not null、任意、初期値空文字
            entity.Property(e => e.ContractorName)
                .HasColumnName("contractor_name")
                .IsRequired()//not null
                .HasMaxLength(20)
                //.HasDefaultValue("")
                .HasComment("契約者氏名");

            //not null、任意、初期値空文字
            entity.Property(e => e.OfficialPosition)
                .HasColumnName("official_position")
                .IsRequired()//not null
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasComment("役職");

            //not null、任意、初期値空文字
            entity.Property(e => e.PostCode)
                .HasColumnName("post_code")
                .IsRequired()//not null
                .HasMaxLength(7)
                .HasDefaultValue("")
                .HasComment("郵便番号");

            //not null、任意、初期値空文字
            entity.Property(e => e.Address)
                .HasColumnName("address")
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasComment("住所");

            //not null、任意、初期値空文字
            entity.Property(e => e.Tel)
                .HasColumnName("tel")
                .IsRequired()
                .HasMaxLength(13)
                .HasDefaultValue("")
                .HasComment("電話番号");

            //not null、任意、初期値空文字
            entity.Property(e => e.Fax)
                .HasColumnName("fax")
                .IsRequired()
                .HasMaxLength(12)
                .HasDefaultValue("")
                .HasComment("ファックス番号");

            //not null、任意、初期値空文字
            entity.Property(e => e.MailAddress)
                .HasColumnName("mail_address")
                .IsRequired()
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasComment("メールアドレス");

            //not null、任意、初期値空文字
            entity.Property(e => e.StartDate)
                .HasColumnName("start_date")
                .IsRequired()
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasComment("利用開始日");

            //not null、任意、初期値空文字
            entity.Property(e => e.EndDate)
                .HasColumnName("end_date")
                .IsRequired()
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasComment("利用終了日");

            //not null、必須
            entity.Property(e => e.CreatedId)
                .HasColumnName("created_id")
                .IsRequired()
                .HasComment("登録者ID");//not null

            //not null、必須、初期値は現在日時
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()//not null
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                //.HasDefaultValueSql("CURRENT_TIMESTAMP(6)")//マイクロ秒まで必要な場合
                .HasColumnType("datetime")//秒までで良い場合
                .HasComment("登録日時");

            //not null、必須
            entity.Property(e => e.UpdatedId)
                .HasColumnName("updated_id")
                .IsRequired()
                .HasComment("更新者ID");//not null

            //not null、必須、初期値は現在日時
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired()//not null
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")//秒までで良い場合
                .HasComment("更新日時");
        }
    }
}