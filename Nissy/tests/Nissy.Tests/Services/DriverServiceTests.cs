using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Nissy.Controllers;
using Nissy.Helpers;
using Nissy.Models;
using Nissy.Models.Entity;
using Nissy.Repositories;
using Nissy.Repositories.Interfaces;
using Nissy.Services;
using Nissy.ViewModels.Driver;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
//.NET6以降標準で入ってるのでusing不要
//using Xunit;

namespace Nissy.Tests.Services
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
    /// </summary>
    public class DriverServiceTests
    {
        #region 共通セットアップ

        private readonly Mock<IDriverRepository> _mockRepository;
        private readonly Mock<ILogger<DriverService>> _mockLogger;
        private readonly DriverService _service;

        /// <summary>
        /// 共通のセットアップはコンストラクタで行う
        /// ※各メソッドごとにコンストラクタ動くので初期化も不要
        /// </summary>
        public DriverServiceTests()
        {
            // Arrange（準備）

            // サービスのコンストラクタに必要な依存関係をモックで用意する

            // モック：Repository作成
            _mockRepository = new Mock<IDriverRepository>();

            // モック：Logger作成
            _mockLogger = new Mock<ILogger<DriverService>>();

            // 本物のリポジトリを使わないために、モックをServiceに渡してインスタンス化
            _service = new DriverService(
                _mockRepository.Object,
                _mockLogger.Object
            );
        }

        #endregion

        #region GetListAsyncのテスト

        //補足：何かしらXunitのアノテーションがないとテストとして認識されないので、[Fact]、[Theory]などは必須でつける
        [Fact]
        public async Task GetListAsync_正常に一覧取得できる()
        {
            // Arrange（準備）

            // InMemoryDBを使ってテストデータ投入するので接続定義
            // ※テスト対象の機能側で「.ToListAsync()」にを使ってる場合、テスト時にLINQ「.AsQueryable()」が対応してなくテストが通らない場合」に必要
            // ※基本はDBつかわずにLINQ「.AsQueryable();」でテストデータセットするが、できないので軽量のメモリ上のDB InMemoryで対応する
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // InMemoryDB を使用する MyContext を生成
            // ※本物DBではなくメモリ上DBへ接続される
            using var context = new MyContext(options);

            // テスト用データ
            // ※カラム設定に依存しないので必須など気にしなくて適当にセットでOK
            context.Drivers.AddRange(
                new DriverEntity
                {
                    Id = 1,
                    Name = "田中"
                },
                new DriverEntity
                {
                    Id = 2,
                    Name = "佐藤"
                }
            );

            // テストデータをInMemory DBに保存
            await context.SaveChangesAsync();

            // 本物Repository
            // ※LINQじゃなくInMemoryDB を使用するので、_mockRepositoryが今回は使えない
            var repository = new DriverRepository(context);

            // Serviceに本物のRepositoryを渡してインスタンス化
            var service = new DriverService(
                repository,
                _mockLogger.Object
            );

            // Act（実行）
            var result = await service.GetListAsync(1);

            // Assert（検証）

            // 一覧が取得できていること
            Assert.NotNull(result);

            // 件数確認
            Assert.Equal(2, result.TotalCount);

            // 1ページ目確認
            Assert.Equal(1, result.CurrentPage);

            // items にデータが入っていること
            Assert.Equal(2, result.items.Count);
        }

        [Fact]
        public async Task GetListAsync_GetQueryで例外発生時_例外を再throwする()
        {
            // Arrange（準備）

            // GetQuery() 実行時に例外発生
            _mockRepository
                .Setup(x => x.GetQuery())
                .Throws(new Exception("DBエラー"));

            // Act & Assert

            // 例外発生確認
            await Assert.ThrowsAsync<Exception>(
                async () => await _service.GetListAsync(1)
            );

            // Logger のエラーログ確認
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
        #endregion

        #region CreateAsyncのテスト

        [Fact]
        public async Task CreateAsync_正常に登録できる場合_trueを返す()
        {
            // Arrange（準備）

            // テスト用入力データ
            var viewModel = new CreateEditViewModel
            {
                Name = "田中",
                BirthDate = "2000/01/01",
                RegistDate = "2024/01/01",
                Other = "備考",
                LicenseNo = "12345",
                ExpireDate = "2030/01/01",
                LicenseDate = "2020/01/01",
                LicenseKind = "普通",
                LicenseCondition = "なし"
            };

            // SaveChangesAsync が正常終了するよう設定
            _mockRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act（実行）
            var result = await _service.CreateAsync(viewModel);

            // Assert（検証）

            // true が返ること
            Assert.True(result);

            // Add が1回呼ばれたこと
            _mockRepository.Verify(
                x => x.Add(It.IsAny<DriverEntity>()),
                Times.Once
            );

            // SaveChangesAsync が1回呼ばれたこと
            _mockRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAsync_ViewModelの値がEntityに正しくセットされる()
        {
            // Arrange（準備）

            var viewModel = new CreateEditViewModel
            {
                Name = "田中",
                BirthDate = "2000/01/01",
                RegistDate = "2024/01/01",
                Other = "備考",
                LicenseNo = "12345",
                ExpireDate = "2030/01/01",
                LicenseDate = "2020/01/01",
                LicenseKind = "普通",
                LicenseCondition = "なし"
            };

            // ViewModel の値が Entity に正しくセットされているか確認するため格納する変数
            DriverEntity? capturedEntity = null;

            // Addした後に登録した値を確認用にセットするためのモック設定
            // ※実行で渡したViewModelの値が、Service内でEntityにセット→その値をcapturedEntityに格納して、後述のAssert（検証）で確認するイメージ
            _mockRepository
                .Setup(x => x.Add(It.IsAny<DriverEntity>()))
                // Addの時に渡されEntityに入った値をCallbackでcapturedEntityに格納する
                .Callback<DriverEntity>(x => capturedEntity = x);

            // SaveChangesAsync が正常終了するよう設定
            _mockRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act（実行）

            await _service.CreateAsync(viewModel);

            // Assert（検証）

            // capturedEntity に値がセットされていることを確認
            Assert.NotNull(capturedEntity);

            // ViewModelの値がEntityに正しくセットされていることを確認

            // 必須項目
            // ※IdはDB登録後に自動採番されるため、今回は確認対象外
            Assert.Equal("田中", capturedEntity.Name);

            // 必須項目：固定値も確認
            // TODO：暫定で固定なので後で調整
            Assert.Equal(1, capturedEntity.ServiceTypeId);
            Assert.Equal(1, capturedEntity.AccountManagerId);

            //任意項目（主なものだけ）
            Assert.Equal("2000/01/01", capturedEntity.BirthDate);
            Assert.Equal("12345", capturedEntity.LicenseNo);
        }

        [Fact]
        public async Task CreateAsync_SaveChangesAsyncで例外発生時_例外を再throwする()
        {
            // Arrange（準備）

            var viewModel = new CreateEditViewModel
            {
                Name = "田中"
            };

            // SaveChangesAsync 実行時に例外発生
            _mockRepository
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("DBエラー"));

            // Act & Assert

            // 例外発生確認
            await Assert.ThrowsAsync<Exception>(
                async () => await _service.CreateAsync(viewModel)
            );

            // Add が呼ばれたこと
            _mockRepository.Verify(
                x => x.Add(It.IsAny<DriverEntity>()),
                Times.Once
            );

            // Logger のエラーログ確認
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        #endregion

        #region GetEditViewModelAsyncのテスト

        [Fact]
        public async Task GetEditViewModelAsync_対象データが存在しない場合_nullを返す()
        {
            // Arrange（準備）

            // モック設定：FindAsync(1) が呼ばれた場合に null を返すよう設定
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync((DriverEntity?)null);

            // Act（実行）

            // GetEditViewModelAsync を実行(モックで設定した内容で仮に行うので本物のDBは操作してない)
            var result = await _service.GetEditViewModelAsync(1);

            // Assert（検証）

            // nullが返ってきたらテスト成功(true)とするアサート
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEditViewModelAsync_対象データが存在する場合_ViewModelを返す()
        {
            // Arrange（準備）

            // テスト用のサンプルデータ
            var driver = new DriverEntity
            {
                Id = 1,
                Name = "田中",
                BirthDate = "2000/01/01",
                RegistDate = "2024/01/01",
                Other = "備考",
                LicenseNo = "12345",
                ExpireDate = "2030/01/01",
                LicenseDate = "2020/01/01",
                LicenseKind = "普通",
                LicenseCondition = "なし"
            };

            // モック設定：FindAsync(1) が呼ばれた場合に対象データを返す
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync(driver);

            // Act（実行）

            // GetEditViewModelAsync を実行(モックで設定した内容で仮に行うので本物のDBは操作してない)
            var result = await _service.GetEditViewModelAsync(1);

            // Assert（検証）

            // サービスから返ってきたことを確認
            Assert.NotNull(result);

            // ViewModelの値がEntityから正しくセットされていることを確認(主なもの)
            Assert.Equal(1, result.Id);
            Assert.Equal("田中", result.Name);
            Assert.Equal("2000/01/01", result.BirthDate);
            Assert.Equal("12345", result.LicenseNo);
        }

        [Fact]
        public async Task GetEditViewModelAsync_FindAsyncで例外発生時_例外を再throwする()
        {
            // Arrange（準備）

            // FindAsync(1) 実行時に例外発生
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ThrowsAsync(new Exception("DBエラー"));

            // Act（実行）
            // Assert（検証）

            // 例外発生確認(Serviceから例外がthrowされることを確認)
            await Assert.ThrowsAsync<Exception>(
                async () => await _service.GetEditViewModelAsync(1)
            );

            // Logger確認
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        #endregion

        #region UpdateAsyncのテスト

        [Fact]
        public async Task UpdateAsync_対象データが存在しない場合_falseを返す()
        {
            // Arrange（準備）

            // モック設定：FindAsync(1) が呼ばれた場合に null を返すよう設定
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync((DriverEntity?)null);

            // 更新用データ
            var viewModel = new CreateEditViewModel
            {
                Name = "田中"
            };

            // Act（実行）

            // UpdateAsync を実行(モックで設定した内容で仮に行うので本物のDBは操作してない)
            var result = await _service.UpdateAsync(1, viewModel);

            // Assert（検証）

            // falseが返ってきたらテスト成功(true)とするアサート
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_正常に更新できる場合_trueを返す()
        {
            // Arrange（準備）

            // テスト用の更新対象データ
            var driver = new DriverEntity
            {
                Id = 1,
                Name = "更新前"
            };

            // 更新用データ
            var viewModel = new CreateEditViewModel
            {
                Name = "更新後"
            };

            // モック設定：FindAsync(1) が呼ばれた場合に対象データを返す
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync(driver);

            // SaveChangesAsync が正常終了するよう設定
            _mockRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act（実行）

            // UpdateAsync を実行(モックで設定した内容で仮に行うので本物のDBは操作してない)
            var result = await _service.UpdateAsync(1, viewModel);

            // Assert（検証）

            // true が返ることを確認
            Assert.True(result);

            // 更新後の値になっていることを確認
            Assert.Equal("更新後", driver.Name);

            // Update が1回呼ばれたことを確認
            _mockRepository.Verify(
                x => x.Update(driver),
                Times.Once
            );

            // SaveChangesAsync が1回呼ばれたことを確認
            _mockRepository.Verify(
                x => x.SaveChangesAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateAsync_ViewModelの値がEntityに正しくセットされる()
        {
            // Arrange（準備）

            // テスト用の更新対象データ
            var driver = new DriverEntity
            {
                Id = 1,
                Name = "更新前"
            };

            // 更新用データ
            var viewModel = new CreateEditViewModel
            {
                Name = "田中",
                BirthDate = "2000/01/01",
                RegistDate = "2024/01/01",
                Other = "備考",
                LicenseNo = "12345",
                ExpireDate = "2030/01/01",
                LicenseDate = "2020/01/01",
                LicenseKind = "普通",
                LicenseCondition = "なし"
            };

            // モック設定：FindAsync(1) が呼ばれた場合に対象データを返す
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync(driver);

            // SaveChangesAsync が正常終了するよう設定
            _mockRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // CreateAsync_ViewModelの値がEntityに正しくセットされる()の時みたいにcapturedEntityを使わない理由
            // - CreateAsync：Service内で新しくEntityを作ってるので、capturedEntityを使ってAddされたEntityを確認する
            // - UpdateAsync：Service内で既存のEntityを取得して、そのEntityの値を更新してるので、capturedEntityは必要なく、直接モックで返すEntityの値が更新されているか確認すればOK

            // Act（実行）

            await _service.UpdateAsync(1, viewModel);

            // Assert（検証）

            // ViewModelの値がEntityに正しくセットされていることを確認

            // 必須項目
            Assert.Equal("田中", driver.Name);

            // 必須項目：固定値も確認
            // TODO：暫定で固定なので後で調整
            Assert.Equal(1, driver.ServiceTypeId);
            Assert.Equal(1, driver.AccountManagerId);
            Assert.Equal(1, driver.UpdatedId);

            // 任意項目（主なものだけ）
            Assert.Equal("田中", driver.Name);
            Assert.Equal("2000/01/01", driver.BirthDate);
        }

        [Fact]
        public async Task UpdateAsync_SaveChangesAsyncで例外発生時_例外を再throwする()
        {
            // Arrange（準備）

            // テスト用の更新対象データ
            var driver = new DriverEntity
            {
                Id = 1,
                Name = "更新前"
            };

            // 更新用データ
            var viewModel = new CreateEditViewModel
            {
                Name = "田中"
            };

            // モック設定：FindAsync(1) が呼ばれた場合に対象データを返す
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync(driver);

            // SaveChangesAsync 実行時に例外発生
            _mockRepository
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("DBエラー"));

            // Act（実行）

            // UpdateAsync 実行時に例外がthrowされることを確認
            await Assert.ThrowsAsync<Exception>(
                async () => await _service.UpdateAsync(1, viewModel)
            );

            // Assert（検証）

            // Update が呼ばれたことを確認
            _mockRepository.Verify(
                x => x.Update(driver),
                Times.Once
            );

            // Logger のエラーログ出力確認(各引数を渡さないといけない)
            // ※実務ではログまでテストしないこともある
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }


        #endregion

        #region DeleteAsyncのテスト
        [Fact]
        public async Task DeleteAsync_対象データが存在しない場合_falseを返す()
        {
            // Arrange（準備）

            // モック設定：FindAsync(1) が呼ばれた場合に null を返すよう設定
            // 本番リポジトリの「await _driverRepository.FindAsync(id)」メソッドに1を渡したときの挙動を再現し、[1]のデータが見つからないケースとして null を返すように設定
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync((Nissy.Models.Entity.DriverEntity?)null);

            // Act（実行）

            // 本物のサービスのDeleteAsyncメソッドに 1 を渡しとモックで挙動をテストする
            // ※モックリポジトリのFindAsyncは、引数に1を渡したときにnullを返すように設定しているため、DeleteAsyncはデータが見つからないケースとしてfalseを返すことが期待される
            var result = await _service.DeleteAsync(1);

            // Assert（検証）

            // falseが返ってきたらテスト成功(true)とするアサート
            Assert.False(result);
        }

        //DeleteAsync_正常に削除できる場合_trueを返す
        [Fact]
        public async Task DeleteAsync_正常に削除できる場合_trueを返す()
        {
            // Arrange（準備）

            // テスト用の削除対象データ
            var driver = new Nissy.Models.Entity.DriverEntity
            {
                Id = 1,
                Name = "テスト"
            };

            // モック設定：FindAsync(1) が呼ばれた場合に対象データを返す
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync(driver);

            // SaveChangesAsync が正常終了(true)するよう設定
            _mockRepository
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act（実行）

            // DeleteAsync を実行(モックで設定した内容で仮に行うので本物のDBは操作してない)
            var result = await _service.DeleteAsync(1);

            // Assert（検証）

            // true が返ることを確認
            Assert.True(result);

            // Remove が1回呼ばれたことを確認
            _mockRepository.Verify(x => x.Remove(driver), Times.Once);

            // SaveChangesAsync が1回呼ばれたことを確認
            _mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SaveChangesAsyncで例外発生時_例外を再throwする()
        {
            // Arrange（準備）

            // テスト用の削除対象データ
            var driver = new Nissy.Models.Entity.DriverEntity
            {
                Id = 1,
                Name = "テスト"
            };

            // モック設定：FindAsync(1) が呼ばれた場合に対象データを返す
            _mockRepository
                .Setup(x => x.FindAsync(1))
                .ReturnsAsync(driver);

            // SaveChangesAsync 実行時に例外発生
            _mockRepository
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("DBエラー"));

            // Act（実行）

            // DeleteAsync 実行時に例外がthrowされることを確認
            await Assert.ThrowsAsync<Exception>(
                async () => await _service.DeleteAsync(1)
            );

            // Assert（検証）

            // Remove が呼ばれたことを確認
            _mockRepository.Verify(x => x.Remove(driver), Times.Once);

            // Logger のエラーログ出力確認(各引数を渡さないといけない)
            // ※実務ではログまでテストしないこともある
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,//Errorレベルでログ出力されることを確認
                    It.IsAny<EventId>(),//EventIdは特に指定してないので、どんな値でもいいようにIt.IsAnyで設定
                    It.IsAny<It.IsAnyType>(),//ログの内容は特に指定してないので、どんな値でもいいようにIt.IsAnyで設定
                    It.IsAny<Exception>(),//Exceptionが渡されたか
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()//ログ文字列変換処理
                ),
                Times.Once
            );
        }
        #endregion
    }
}