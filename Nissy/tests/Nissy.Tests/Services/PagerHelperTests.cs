using Microsoft.EntityFrameworkCore;
using Nissy.Helpers;
using Xunit;

namespace Nissy.Tests.Helpers
{
    /// <summary>
    /// テスト方針：サービスに存在する機能だけ最低限テストする
    /// 
    /// ・正常系：想定通り成功するパターン
    /// ・異常系：例外発生時の挙動
    /// ・条件分岐：if分岐や対象データなし
    /// ・セキュリティ関連：認可、ログイン状態など
    /// ・外部連携：外部APIやDBアクセスのモックを使ったテスト
    /// 
    /// ※フレームワーク標準機能は対象外
    /// ※ログイン系、外部連携：対象機能にチェックなどない限り不要
    /// 
    /// ※ページャのテスト作成方針補足
    /// データ加工のみなので、正常系と条件分岐（データ0件）を中心にテストケースを作成
    /// データ0件、ページごとに動いてれば異常系は特に想定されないため、今回は作成しない
    /// </summary>
    public class PagerHelperTests
    {
        // PagerHelperTests 専用のテスト用DBContextクラス
        // ※Helperは共通機能なので、本物の MyContext (その他機能)に依存しないよう、テスト用に簡易的な DbContext をテスト用に定義
        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options)
            {
            }

            // テスト用の Drivers テーブル
            // ※InMemoryDB 上で TestDriver データを操作するために定義
            // ※ページャーはいろんなデータで使えるように汎用的になってるので、今回は最初に出来てる
            public DbSet<TestTable> Test { get; set; }
        }

        // テスト用Entity
        public class TestTable
        {
            public int Id { get; set; }
        }


        [Fact]
        public async Task CreateAsync_正常にページャー生成できる()
        {
            // Arrange（準備）

            // InMemoryDB作成(PagerがToListAsyncのため)
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);

            // テストデータ投入
            context.Test.AddRange(
                new TestTable { Id = 1},
                new  TestTable { Id = 2},
                new  TestTable { Id = 3},
                new  TestTable { Id = 4},
                new  TestTable { Id = 5},
                new  TestTable { Id = 6}
            );

            await context.SaveChangesAsync();

            // Act（実行）

            var result = await PagerHelper.CreateAsync(
                context. Test.OrderBy(x => x.Id),//テストデータをID順で取得
                page: 1,
                pageSize: 5
            );

            // Assert（検証）

            // 総データ件数が正しく取得できているか
            Assert.Equal(6, result.TotalCount);

            // 現在のページが正しく取得できているか
            Assert.Equal(1, result.CurrentPage);

            // 総ページ数が正しく取得できているか
            Assert.Equal(2, result.TotalPage);

            // 表示開始位置が正しく取得できているか
            Assert.Equal(1, result.DispStart);

            // 表示終了位置が正しく取得できているか(1ページ目なので5)
            Assert.Equal(5, result.DispEnd);

            // 表示データが正しく取得できているか (1ページ分の取得数)
            Assert.Equal(5, result.items.Count);
        }

        [Fact]
        public async Task CreateAsync_最終ページの端数が正しく取得できる()
        {
            // Arrange（準備）

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);

            // 6件 / pageSize=5 → 2ページ目は1件だけになるように6個のテストデータを投入
            context.Test.AddRange(
                new TestTable { Id = 1 },
                new TestTable { Id = 2 },
                new TestTable { Id = 3 },
                new TestTable { Id = 4 },
                new TestTable { Id = 5 },
                new TestTable { Id = 6 }
            );
            await context.SaveChangesAsync();

            // Act（実行）

            // Act：2ページ目を取得
            var result = await PagerHelper.CreateAsync(
                context.Test.OrderBy(x => x.Id),
                page: 2,
                pageSize: 5
            );

            // Assert（検証）

            // 総データ件数が正しく取得できているか
            Assert.Equal(6, result.TotalCount);

            // 現在のページが正しく取得できているか
            Assert.Equal(2, result.CurrentPage);

            // 総ページ数が正しく取得できているか
            Assert.Equal(2, result.TotalPage);

            // 表示開始位置が正しく取得できているか
            Assert.Equal(6, result.DispStart);

            // 表示終了位置が正しく取得できているか(PazeSizeは5だが最終ページなので5+1=6になる)
            Assert.Equal(6, result.DispEnd);

            // 表示データが正しく取得できているか (1ページ分の取得数)
            // 最終ページのデータは1件だけ
            Assert.Single(result.items);
        }

        [Fact]
        public async Task CreateAsync_データ0件の場合()
        {
            // Arrange（準備）

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);

            // Act（実行）

            var result = await PagerHelper.CreateAsync(
                context.Test.OrderBy(x => x.Id),
                page: 1,
                pageSize: 5
            );

            // Assert（検証）

            // データ0件の検証
            Assert.Empty(result.items);
        }
    }
}