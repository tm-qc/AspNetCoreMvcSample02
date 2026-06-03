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
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUserEntity>
    {
        //TODO:あくまでサンプル。適宜調整
        public void Configure(EntityTypeBuilder<AppUserEntity> entity)
        {
            entity.ToTable("app_user", t => t.HasComment("アプリユーザーマスタ"));

            // ---- 主キー ----
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd() // オートインクリメント
                .HasComment("ID");

            // ---- FK: アカウント管理マスタID ----
            //not null、必須
            entity.Property(e => e.AccountManagerId)
                .HasColumnName("account_manager_id")
                .IsRequired() // not null
                .HasComment("アカウント管理マスタID");

            // アカウント管理マスタ.idとFK
            // 1:多のリレーションを定義するためのメソッド
            entity.HasOne<AccountManagerEntity>()
                .WithMany()
                .HasForeignKey(e => e.AccountManagerId)
                //親を削除しようとするとエラー
                .OnDelete(DeleteBehavior.Restrict);

            // ---- ログインコード ----
            //not null、必須
            entity.Property(e => e.LoginCode)
                .HasColumnName("login_code")
                .IsRequired() // not null
                .HasMaxLength(10)
                .HasComment("ログインコード");

            // ---- ログインPW ----
            //not null、必須
            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired() // not null
                .HasMaxLength(255)
                .HasComment("パスワード(hash)");

            // ---- 氏名 ----
            //not null、必須
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired() // not null
                .HasMaxLength(20)
                .HasComment("氏名");

            // ---- 氏名ふりがな ----
            //not null、必須
            entity.Property(e => e.NameKana)
                .HasColumnName("name_kana")
                .IsRequired() // not null
                .HasMaxLength(30)
                .HasComment("氏名ふりがな");

            // ---- サビ管フラグ ----
            //not null、任意、初期値0
            entity.Property(e => e.ServiceManagerFlag)
                .HasColumnName("service_manager_flag")
                .IsRequired() // not null
                .HasDefaultValue(0)
                .HasComment("サービス管理責任者フラグ（1:サービス管理責任者 2:補助サービス責任者）");

            // ---- リスト表示フラグ ----
            //not null、任意、初期値0
            entity.Property(e => e.IncludeListFlag)
                .HasColumnName("include_list_flag")
                .IsRequired() // not null
                .HasDefaultValue(0)
                .HasComment("リスト表示フラグ（1:リストに含む）");

            // ---- 管理者区分 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.AdminClassification)
                .HasColumnName("admin_classification")
                .IsRequired() // not null
                .HasMaxLength(2)
                .HasDefaultValue("")
                .HasComment("管理者区分（01:一般 02:個別支援計画作成者 99:システム管理者）");

            // ---- 入社日 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.JoiningDate)
                .HasColumnName("joining_date")
                .IsRequired() // not null
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasComment("入社日");

            // ---- 退社日 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.RetirementDate)
                .HasColumnName("retirement_date")
                .IsRequired() // not null
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasComment("退社日");

            // ---- 次回研修アラーム日付1 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.AlarmDate1)
                .HasColumnName("alarm_date_1")
                .IsRequired() // not null
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasComment("次回研修アラーム日付1");

            // ---- 次回研修アラーム名称1 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.AlarmName1)
                .HasColumnName("alarm_name_1")
                .IsRequired() // not null
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasComment("次回研修アラーム名称1");

            // ---- 次回研修アラーム日付2 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.AlarmDate2)
                .HasColumnName("alarm_date_2")
                .IsRequired() // not null
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasComment("次回研修アラーム日付2");

            // ---- 次回研修アラーム名称2 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.AlarmName2)
                .HasColumnName("alarm_name_2")
                .IsRequired() // not null
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasComment("次回研修アラーム名称2");

            // ---- 次回研修アラーム日付3 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.AlarmDate3)
                .HasColumnName("alarm_date_3")
                .IsRequired() // not null
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasComment("次回研修アラーム日付3");

            // ---- 次回研修アラーム名称3 ----
            //not null、任意、初期値空文字
            entity.Property(e => e.AlarmName3)
                .HasColumnName("alarm_name_3")
                .IsRequired() // not null
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasComment("次回研修アラーム名称3");

            // ---- 表示順 ----
            //not null、必須
            entity.Property(e => e.DisplayOrder)
                .HasColumnName("display_order")
                .IsRequired() // not null
                .HasComment("表示順");

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