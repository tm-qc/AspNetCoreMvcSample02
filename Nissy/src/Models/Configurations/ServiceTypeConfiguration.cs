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
    public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceTypeEntity>
    {
        //TODO:あくまでサンプル。適宜調整
        public void Configure(EntityTypeBuilder<ServiceTypeEntity> entity)
        {
            entity.ToTable("service_type", t => t.HasComment("サービス種別マスタ"));

            // ---- 主キー ----
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd() // オートインクリメント
                .HasComment("ID");

            // ---- サービス区分 ----
            //not null、必須
            entity.Property(e => e.ServiceDivision)
                .HasColumnName("service_division")
                .IsRequired() // not null
                .HasMaxLength(2)
                .HasComment("サービス区分");

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