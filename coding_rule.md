# 初心者向け ASP.NET Core MVC コーディング規約

## はじめに
作者が、Visual StudioやC#、ASP.NET CORE作成経験なく、詳しくないですが、
今後できるだけ改修、分析で困らないように、共通認識で守った方が良さそうことを、簡単に記載しました。

簡単とはいえ、すべて守るのも難しいとは思います。
極力認識あわせて作成進めれたら幸いです。

一番大事なのは知らない人が見たときにも、わかるようにコードを記載する思いやりの意識かなと思います。

**何かおかしな点があれば、遠慮なく指摘・改善提案をお願いします。**  
**コード形成など、自動管理で便利なプラグインがあれば教えてください。**

## 0. 共通

### 命名の意識
- どんな機能か予測ができる名前にする
- マジックナンバーを使わない
そのうえで命名規則を守る

### コメントについて
多すぎるコメントは好まれませんが、読んだ人がわからないが一番よくありません。  
実装背景、補足、機能の説明など、コードを見てもパッとわからないことを中心に、
今後誰かが見たときに困らないように記載しておきましょう

基本は「 XMLドキュメントコメント（///）」を使うといいと思います。  
「///」で関数に対して以下のようなひな形ができます。

```

/// <summary>
/// 商品情報を取得します
/// </summary>
/// <param name="productId">商品ID</param>
/// <returns>商品情報。見つからない場合はnull</returns>
public async Task<Product> GetProductAsync(int productId)
{
    // 実装
}

```

### 1クラス1責務
コードが長すぎると以下の弊害が生まれます

- 仕様把握、解析、改修ができない
- コードレビューができずに案件が進まない
- テスト実装の範囲が広すぎてできない

人がパッと見てわかる程度の短いコード、分離を意識しましょう。

「1クラス1責務」

これを原則に1ファイルのコードが極力短く、依存性がなく、1クラスの役割を1つに絞る意識が大事です。  
※1クラスに複数メソッドはOK。適切な分離で少ないのが一番べスト。

### var
型が明確な場合のみ使用

## 1. 命名規則（Naming Conventions）

### クラス
- **PascalCase** を使用
- クラス名は基本単数
- モデル（Entity） → 単数形
  - 例：`Product`, `Order`, `Customer`
- Controller → クラス名 + `Controller`
  - 例：`ProductsController`
- Service → クラス名 + `Service`
  - 例：`OrderService`
- Repository → クラス名 + `Repository`
  - 例：`CustomerRepository`

### メソッド・プロパティ・定数
- **PascalCase** を使用
- 名前は **機能が予測できるもの** にする。
  - ❌ 悪い例：`DoWork()`, `Data1`,`public const int MaxRetryCount1 = 3;`
  - ✅ 良い例：`CalculateDiscount()`, `SaveOrder()`,`public const int MaxRetryCount = 3;`

### 変数・引数
- **camelCase** を使用
  - 例：`itemCount`, `customerName`

### モデルの命名
- **PascalCase** を使用
- **主キー** → `Id`（例：`Product.Id`）
- **外部キー** → 親クラス名 + `Id`（例：`CustomerId`）
- **ナビゲーションプロパティ(リレーション)**
  - 親 → 単数形（例：`Customer`）
  - 子 → 複数形（例：`Orders`）

### ファイル名
- **PascalCase** を使用
  - 例：`Create.cshtml`
  
クラス名と同じ or ViewはASP.NET Core標準がPascalCaseを推奨

### 補足
#### DBのテーブル名について
テーブル名は基本複数形で作成が良さそうです。 

理由は以下

- データは複数のレコードを格納するため
- モデル名（エンティティクラス）が単数形の名前は複数形のテーブル名に自動マッピングするので指定不要
- データベースのテーブル名が単数→モデル名が複数形の関係で書けないときは、アノテーションで指定する  
例）[Table("Sample")] 

#### DBのカラム名について
C#では以下の理由でPascalCaseが主流だそうです  

- カラム名(プロパティ)はC#の場合PascalCaseが主流
- PascalCaseIdで書くと外部キーを親子間で自動で認識してくれる
- snake_case_idで書くと外部キーを認識してくれないので[ForeignKey("SampleId")]とアノテーションをコードに追記する必要がある

```
例）	
- modelクラス：Sample
- 親テーブル：Samples
- 親テーブルのIDカラム：Id
- 子テーブルの外部キーカラム：SampleId
```


## 2. MVC基本フォルダ構成

```

/Controllers   → 画面遷移・入力受付
/Services      → ビジネスロジック
/Repositories  → データアクセス
/Models        → データモデル
/Views         → 画面(.cshtml)

```

- フォルダ名は基本的には複数形
- Controller は処理を持たず、Service を呼び出す
- レイヤーを分けて責務を明確化

### MVC+Service+Repository 流れのイメージ
```

Views
↓↑
Controllers(機能の呼び出し)
↓↑
Services (ビジネスロジック。業務処理、複雑な計算、トランザクション管理など)   
↓↑
Repositories (データ取得・保存（DBアクセス、外部API連携など）)
↓↑
Models (モデル定義のみ)
↓↑
DB

※ViewModelやEntityなどへのバインドはServiceで行います。  
- Viewに渡す値：ViewModelにバインド  
- DBに登録する値：取得した更新データ or Entityにバインド

```

## 3. 依存性注入（DI）
ServiceやRepositoryを追加する場合は、Program.csで登録してDIとして使います。

### Program.cs での登録
```csharp

services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IProductService, ProductService>();

```

### Controller でのコンストラクタでDI定義の例
```csharp

public class ProductsController : Controller
{
    private readonly IProductService _service;
    
    //コンストラクタでインターフェース経由で参照
    public ProductsController(IProductService service) 
        => _service = service;
}

```

### DIのメリット
- 変更に強い	
- インターフェース経由なので、メソッドの参照ミスや呼び出し、記載漏れを防げる	
- 依存関係がコンストラクタで明確	
- Program.csで定義一覧ができる	
- テストが簡単（Fakeテスト実装時の差し替えが簡単になる）	

## 4. Controller の設計

### 非同期アクション
```csharp
public async Task<IActionResult> Index()
{
    var products = await _service.GetAllAsync();
    return View(products);
}
```

### 設計原則
- Fat Controller を避け、処理は必ず Service に委譲
- ルーティングは属性ルートを使用（特に何も使わない？ほかにいい方法あったら教えてください）

```csharp
[Route("products/{id}")]
public async Task<IActionResult> Details(int id) 
{ 
    // ... 
}
```

## 5. Service / Repository の役割

- **Repository**：DB操作のみ
- **Service**：ビジネスロジック、Repository参照

```csharp

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    
    public ProductService(IProductRepository repo) 
        => _repo = repo;
    
    public async Task<Product> GetProductAsync(int id)
        => await _repo.FindByIdAsync(id);
}

```

## 6. リレーションを用いた結合について
原則「Include」を使うこと

### JoinとIncludeの違い

1. SQLでのJOIN構文

```
SELECT tbl1.*, tdl2.*
FROM table1 tbl1
- LEFT JOINで履歴がない場合はNULLで取得
LEFT JOIN table2 tdl2
  ON tbl1.Id = tdl2.tbl1Id
WHERE その他条件があれば記載;
```

2. ASP.NET Core MVC（LINQ + Include）構文  
※モデルでリレーション設定は済みとする

```
var result = await _context.Table1
    .Include(tbl1 => tbl1.tdl2)
    .Where(その他条件があれば記載)
    .ToListAsync();
```

※LEFT JOINと同様の動き  
※INNER JOINにしたい場合は、`.Where(tbl1 => tbl1.tdl2.Any())`を追加する

# Includeがいい理由
- リレーション設定してる場合、JOINだとリレーションを設定した意味がなくなる  
(紐づけが結局手動になり不整合の原因になる、 ナビゲーションプロパティ構造で子のデータが持てない)
- Includeを使うことで、リレーション設定に基づいて自動で紐づけて取得してくれる  
（内部的にはLEFT JOINと同じ）
- JOINだと手動で条件を指定する必要がありミスの元になる
- 型がモデルに基づいているため、View()にそのまま渡せて利用が簡単
- Joinだと型が匿名型になりモデルじゃないため、View()にそのまま渡せないので型変換=ViewModelを別途用意する必要があるらしい
- **Include使えばViewModel作成しなくていい**
- 子をナビゲーションプロパティ(Includeで取得した子データ)として自然に参照できる。
- Includeはリレーション設定が必要なので、リレーション設定を忘れない
- Includeしておけば、子からループで取り出す場合にN+1問題が起きない

※Includeしない or JOIN後は親オブジェクトの中に子データが格納されない = ナビゲーションプロパティ構造がない  
  そのため、後で子にアクセスすると毎回DBクエリが走りN+1問題が起きる可能性が高い

※JOINは特殊なケースでどうしようもないときにカスタムで使うのが一般的とのこと

## 7. モデルとバリデーション
モデルは処理は書かない、モデル定義のみ記載する

### DataAnnotation を使用したバリデーション
（バリデーションはほかにいい方法あったら教えてください）

```csharp

public class Product
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Range(1, 9999)]
    public decimal Price { get; set; }
}

```

### Controller 側でのチェック
- Controller で `ModelState.IsValid` をチェック

## 8. コーディングスタイル
- インデントには 4 つのスペースを使用します。 タブは使用しないでください。

## 9. エラーハンドリングとロギング

- Service 層で `try-catch` を行い、`ILogger` でログ出力
- Controller は原則 Service に委譲

## 10. テストしやすさ（Testability）

- Service / Repository はインターフェース経由で設計
- モック化、Fake差し替えを可能にしてユニットテストを容易に

### 補足
- Fakeクラス：自分で作成するテスト用クラス
- Mockクラス：モックフレームワークで使える。自動生成で簡単に実装できるテスト用のクラス(テスト不要の関連クラスをtrue/falseで返すなど)

## 11. GitHub
GitHubの使い方はPJで全然違いますが、今まで見てきた感じを記載します。  
現PJでやりやすい形を検討する際の参考になれば幸いです。

### ブランチ名
- main：本番環境
- develop：開発環境。mainからできたものでfeatureが集約される
- feature：新機能、改修。developから作られる
- hotfix：軽いバグの修正。developから作られる
- doc：ドキュメント追加
- test：個人用のテストなどを行う。マージはしない

※GitHub flow(簡易)とGit flow(複雑)でPJによって良しなに改造して使うことが多かった 

### ブランチの命名規則
```
○○/チケット(案件)番号/機能名/任意  
```
例）feature/No01/UserCreate/〇〇

### 開発初期段階で使うことが多いブランチ
- main
- feature
- hotfix
- doc
- test

※リリース前なのでdevelopがない

### ブランチ、改修内容を区切るポイント
極力改修後のコード、内容が短くなるように分ける。  
変更点が多いと、動作確認、コードレビューができなくなる

※パッと見で何したかわかるレベルが理想  
※レビュー → マージ → 改修を軽快に回すイメージ  
※レビュワーの負担にならないようにする

### プルリク(レビュー依頼)のフォーマット

```md

# 概要
[チケット(案件)番号] に対応しました。
[機能の概要を1-2文で簡潔に説明]

# 改修内容
- [主な変更点1]
- [主な変更点2]
- [主な変更点3]

## 動作確認方法
1. [確認手順1]
2. [確認手順2]
3. [確認手順3]

※ 必要に応じてスクリーンショット or 動画も検討  
※ レビュワーの負担にならないようにする

## レビューポイント
- [特に確認してほしい点1]
- [特に確認してほしい点2]

# その他補足

```

### 開発初期段階の流れ

```
1. mainからpull  
↓
2. ブランチ作成  
↓
3. プルリク依頼  
↓
4. レビュー、マージ  
↓
5. 1に戻り改修  
```

### コードレビュー
ASP.NET COREに慣れてない人もいるので、とりあえず以下の方針

- 動くか確認
- コードについてはわかる範囲で確認

## 12. 参考リンク

### 公式ドキュメント
- **C# コーディング規約（Microsoft公式）**
  - [https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
  
- **ASP.NET Core MVC 公式ガイド**
  - [https://learn.microsoft.com/ja-jp/aspnet/core/mvc/overview](https://learn.microsoft.com/ja-jp/aspnet/core/mvc/overview)