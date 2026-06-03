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
    public class DriverConfiguration : IEntityTypeConfiguration<DriverEntity>
    {
        //TODO:あくまでサンプル。適宜調整
        public void Configure(EntityTypeBuilder<DriverEntity> entity)
        {
            entity.ToTable("driver", t => t.HasComment("運転者マスタ"));

            // ---- 主キー ----
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd() // オートインクリメント
                .HasComment("ID");

            // ---- サービス種別ID ----
            //not null、必須
            entity.Property(e => e.ServiceTypeId)
                .HasColumnName("service_type_id")
                .IsRequired() // not null
                .HasComment("サービス種別ID");

            // FK：サービス種別マスタ.id(1:多)
            entity.HasOne<ServiceTypeEntity>()
                .WithMany()
                .HasForeignKey(e => e.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- FK: アカウント管理マスタID ----
            //not null、必須
            //TODO:予定
            entity.Property(e => e.AccountManagerId)
                .HasColumnName("account_manager_id")
                .IsRequired() // not null
                .HasComment("アカウント管理マスタID");

            // FK：アカウント管理マスタ.idとFK
            // ■リレーション設定
            // 子側のテーブル定義にFK設定記載するだけでOK。以下のように整合性を保つために行う
            //  - 子が取り残されないように、親だけ消す場合にエラーが出るようにする
            //  - deleted_at カラムや is_deleted フラグを使ってソフトデリートで対応だとエラーにならない
            //
            // HasOne<親のEntity名>.WithMany():1対多のリレーションを定義するためのメソッド
            // HasOne<親のEntity名>.WithOne():1対1のリレーションを定義するためのメソッド
            entity.HasOne<AccountManagerEntity>()
                .WithMany()
                .HasForeignKey(e => e.AccountManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- 氏名 ----
            //not null、必須
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired() // not null
                .HasMaxLength(30)
                .HasComment("氏名");

            // ---- 生年月日 ----
            //not null、任意、初期値空文字
            //TODO:文字列は暫定。日付にできるなら日付にする
            entity.Property(e => e.BirthDate)
                .HasColumnName("birth_date")
                .IsRequired() // not null
                .HasMaxLength(11)
                .HasDefaultValue("")
                .HasComment("生年月日");

            // ---- 運転者登録日 ----
            //not null、任意、初期値空文字
            //TODO:文字列は暫定。日付にできるなら日付にする
            entity.Property(e => e.RegistDate)
                .HasColumnName("regist_date")
                .IsRequired() // not null
                .HasMaxLength(11)
                .HasDefaultValue("")
                .HasComment("運転者登録日");

            // ---- その他 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.Other)
                .HasColumnName("other")
                .IsRequired() // not null
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasComment("その他");

            // ---- 運転免許証番号 ----
            //not null、任意、初期値空文字
            //TODO:文字列は暫定。intにできるならintにする
            entity.Property(e => e.LicenseNo)
                .HasColumnName("license_no")
                .IsRequired() // not null
                .HasMaxLength(12)
                .HasDefaultValue("")
                .HasComment("運転免許証番号");

            // ---- 有効期限 ----
            //not null、任意、初期値空文字
            //TODO:文字列は暫定。日付にできるなら日付にする
            entity.Property(e => e.ExpireDate)
                .HasColumnName("expire_date")
                .IsRequired() // not null
                .HasMaxLength(11)
                .HasDefaultValue("")
                .HasComment("有効期限");

            // ---- 免許年月日 ----
            //not null、任意、初期値空文字
            //TODO:文字列は暫定。日付にできるなら日付にする
            entity.Property(e => e.LicenseDate)
                .HasColumnName("license_date")
                .IsRequired() // not null
                .HasMaxLength(11)
                .HasDefaultValue("")
                .HasComment("免許年月日");

            // ---- 免許の種類 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.LicenseKind)
                .HasColumnName("license_kind")
                .IsRequired() // not null
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasComment("免許の種類");

            // ---- 免許の条件 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.LicenseCondition)
                .HasColumnName("license_condition")
                .IsRequired() // not null
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasComment("免許の条件");

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
            //テーブル定義書に「ユーザーマスタ.IDとFK」とあるが不要とする
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