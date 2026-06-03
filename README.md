## 基盤の元となるサンプル
簡単に以下を作成

- MCV+Services+Repositories構造
- Migrations+Seeders
- Helpers
- ログイン
- CRUD基盤
- 単体テスト(CRUD+認証)

## 起動準備
- DB作成
- マイグレーションでテーブル作成
- appsettings.Developmentでサンプルデータ投入をtrueにして起動  
  ※起動後falseに戻したら以後データ初期化はされないので、必要に応じて戻す

## 補足

### マイグレーションコマンド
1.毎回マイグレーションファイル作成  
dotnet ef migrations add 目的が判る名前

2.DBに適用しテーブル作成  
dotnet ef database update