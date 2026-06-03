using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nissy.Models.Entity;
/*
 ASP.NET Coreの規約に従い、クラス名は単数形、テーブル名は複数形で命名
 ※ASP.NET Coreの仕様として複数形のテーブル名に自動でマッピングされるため、クラス名は単数形で定義
 ※正しい複数形じゃない場合や他の形式の場合は、[Table("テーブル名")]属性を使用してテーブル名を明示的に指定する必要がある

■テーブル設定は基本的にFluent APIで行うことが推奨される
- Entityファイルにデータアノテーション書いてもできるが、ユニーク、INDEXなどできないこともある
- Fluent APIでの方が、優先度高く上書きされるので、基本的にFluent APIを使う
- Entityファイルはカラム定義+型指定だけでOK
  ※型指定はEntityから参照される
  ※ただしstringは ?(null許容)を付けないとC#の警告が出るので、
    テーブルカラム設定をnot nullにする場合は
    - C#警告回避用に「 = string.Empty;」で空文字を初期値にセット
    - テーブルカラム用の初期値設定はFluent API側で.HasDefaultValue("");でセット（空文字の例）
　　などの初期値設定の対応をする
 ※型だけで?も不要：バリデーションで必須となるので結局 ? は必須以外はEntity時点でもつけたほうがいい

■カラム設定（Data Annotationsの場合）
オートインクリメント：int + [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
PK ：[Key]
型：定義で指定
桁数:アノテーションに(100)で指定(例：[MaxLength(100)]) 
not null：型に ? つけない(例：string)
null許容：型に ? つける(例：string?)
decimalの精度：型指定だけじゃなく精度を指定する必要がある（例：decimal(18, 2) → [Column(TypeName = "decimal(18, 2)")]）
初期値：代入する（今の時間例：= DateTime.UtcNow、空文字例： = string.Empty）
ユニーク：Data AnnotationsではできないのでFluent APIでする
INDEX：Data AnnotationsではできないのでFluent APIでする

■カラム設定（Fluent APIの場合：Data Annotationsより優先度高いのでこちらで最終上書き）
DBのコンテキストファイルで読み込み定義する方法
- テーブルごとにファイル分けして「Models\Configurations\」に配置してる
- DBコンテキスト(MyContext)のOnModelCreatingで一括で読み込む


 */
[Table("account_manager")]
public class AccountManagerEntity
{
    public int Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContractorName { get; set; } = string.Empty;
    public string? OfficialPosition { get; set; } = string.Empty;
    public string? PostCode { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? Tel { get; set; } = string.Empty;
    public string? Fax { get; set; } = string.Empty;
    public string? MailAddress { get; set; } = string.Empty;
    public string? StartDate { get; set; } = string.Empty;
    public string? EndDate { get; set; } = string.Empty;
    public int CreatedId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpdatedId { get; set; }
    public DateTime UpdatedAt { get; set; }
}