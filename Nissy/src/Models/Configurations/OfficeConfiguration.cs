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
    public class OfficeConfiguration : IEntityTypeConfiguration<OfficeEntity>
    {
        //TODO:あくまでサンプル。適宜調整
        public void Configure(EntityTypeBuilder<OfficeEntity> entity)
        {
            entity.ToTable("office", t => t.HasComment("事業所マスタ"));

            // ---- 主キー ----
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd() // オートインクリメント
                .HasComment("ID");

            // ---- FK: アカウント管理マスタID ----
            //not null、必須
            //TODO:予定
            entity.Property(e => e.AccountManagerId)
                .HasColumnName("account_manager_id")
                .IsRequired() // not null
                .HasComment("アカウント管理マスタID");

            // アカウント管理マスタ.idとFK
            // ■リレーション設定
            // 子側のテーブル定義にFK設定記載するだけでOK。以下のように整合性を保つために行う
            //  - 子が取り残されないように、親だけ消す場合にエラーが出るようにする
            //  - deleted_at カラムや is_deleted フラグを使ってソフトデリートで対応だとエラーにならない
            //
            // HasOne<親のEntity名>.WithMany():1対多のリレーションを定義するためのメソッド
            // HasOne<親のEntity名>.WithOne():1対1のリレーションを定義するためのメソッド
            entity.HasOne<AccountManagerEntity>()
                .WithMany()
                // ■キーの設定
                // 子のFKカラム名を指定(親のEntity+Idと基本的になる)
                //.HasForeignKey<Office>(e => e.AccountManagerId)//1:1の場合は1:多と書き方少し違う
                .HasForeignKey(e => e.AccountManagerId)
                // ■削除制御の設定
                // Restrict:親を削除しようとするとエラー
                // Cascade:親削除で子も削除
                //
                //↓基本使わないと思う
                // SetNull:親削除で子のFKをNULLにする（null許容の場合のみ）
                // NoAction:EF Core側は何もしない→MySQLのデフォルト挙動に任せる(MySQLではRestrictと同じ)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- 会社名 ----
            //not null、必須
            entity.Property(e => e.CompanyName)
                .HasColumnName("company_name")
                .IsRequired() // not null
                .HasMaxLength(50)
                .HasComment("会社名");

            // ---- 事業所名 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.OfficeName)
                .HasColumnName("office_name")
                .IsRequired() // not null
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasComment("事業所名");

            // ---- 代表者氏名 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.DelegateName)
                .HasColumnName("delegate_name")
                .IsRequired() // not null
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasComment("代表者氏名");

            // ---- 役職 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.OfficialPosition)
                .HasColumnName("official_position")
                .IsRequired() // not null
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasComment("役職");

            // ---- 郵便番号 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.PostCode)
                .HasColumnName("post_code")
                .IsRequired() // not null
                .HasMaxLength(7)
                .HasDefaultValue("")
                .HasComment("郵便番号");

            // ---- 住所 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.Address)
                .HasColumnName("address")
                .IsRequired() // not null
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasComment("住所");

            // ---- 電話番号 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.Tel)
                .HasColumnName("tel")
                .IsRequired() // not null
                .HasMaxLength(13)
                .HasDefaultValue("")
                .HasComment("電話番号");

            // ---- ファックス番号 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.Fax)
                .HasColumnName("fax")
                .IsRequired() // not null
                .HasMaxLength(12)
                .HasDefaultValue("")
                .HasComment("ファックス番号");

            // ---- メールアドレス ----
            //not null、任意、初期値空文字
            entity.Property(e => e.MailAddress)
                .HasColumnName("mail_address")
                .IsRequired() // not null
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasComment("メールアドレス");

            // ---- 登録者ID ----
            //not null、必須
            entity.Property(e => e.CreatedId)
                .HasColumnName("created_id")
                .IsRequired() // not null
                .HasComment("登録者ID");

            // ---- 登録日時 ----
            //not null、必須、初期値は現在日時
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired() // not null
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                //.HasDefaultValueSql("CURRENT_TIMESTAMP(6)") // マイクロ秒まで必要な場合
                .HasColumnType("datetime") // 秒までで良い場合
                .HasComment("登録日時");

            // ---- 更新者ID ----
            //not null、必須
            entity.Property(e => e.UpdatedId)
                .HasColumnName("updated_id")
                .IsRequired() // not null
                .HasComment("更新者ID");

            // ---- 更新日時 ----
            //not null、必須、初期値は現在日時
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired() // not null
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime") // 秒までで良い場合
                .HasComment("更新日時");
        }
    }
}