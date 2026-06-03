using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Nissy.Models.Entity;
using Nissy.Repositories.Interfaces;
using Nissy.Services;

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
    /// </summary>
    public class AuthServiceTests
    {
        #region 共通セットアップ

        private readonly Mock<IAppUserRepository> _mockRepository;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _service;

        /// <summary>
        /// 共通のセットアップはコンストラクタで行う
        /// ※各メソッドごとにコンストラクタ動くので初期化不要
        /// </summary>
        public AuthServiceTests()
        {
            // Arrange（準備）

            // モック：Repository作成
            _mockRepository = new Mock<IAppUserRepository>();

            // モック：Logger作成
            _mockLogger = new Mock<ILogger<AuthService>>();

            // Service生成
            _service = new AuthService(
                _mockRepository.Object,
                _mockLogger.Object
            );
        }

        #endregion

        #region AuthenticateAsyncのテスト

        [Fact]
        public async Task AuthenticateAsync_ユーザが存在しない場合_nullを返す()
        {
            // Arrange（準備）

            // ユーザが存在しないパターンでモックの戻り値を設定
            _mockRepository
                .Setup(x => x.GetByLoginCodeAsync("test"))
                .ReturnsAsync((AppUserEntity?)null);

            // Act（実行）

            // 認証処理を実行
            var result = await _service.AuthenticateAsync(
                "test",
                "password"
            );

            // Assert（検証）

            // 結果がnullであることを検証
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_PasswordHashを持っていない場合_nullを返す()
        {
            // Arrange（準備）

            // PasswordHashを持っていないパターンでユーザを作成
            var user = new AppUserEntity
            {
                LoginCode = "test",
                PasswordHash = string.Empty
            };

            // モックの戻り値を設定
            _mockRepository
                .Setup(x => x.GetByLoginCodeAsync("test"))
                .ReturnsAsync(user);

            // Act（実行）

            // 認証処理を実行
            var result = await _service.AuthenticateAsync(
                "test",
                "password"
            );

            // Assert（検証）

            // 結果がnullであることを検証
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_パスワード不一致の場合_nullを返す()
        {
            // Arrange（準備）

            // 検証用にユーザを作成
            var user = new AppUserEntity
            {
                LoginCode = "test"
            };

            // パスワードをハッシュ化してユーザにセット
            var hasher = new PasswordHasher<AppUserEntity>();
            user.PasswordHash = hasher.HashPassword(
                user,
                "correct-password"//成功するパスワード
            );

            // モックの戻り値を設定
            _mockRepository
                .Setup(x => x.GetByLoginCodeAsync("test"))
                .ReturnsAsync(user);

            // Act（実行）

            // 認証処理を実行（間違ったパスワードを渡す）
            var result = await _service.AuthenticateAsync(
                "test",
                "wrong-password"
            );

            // Assert（検証）

            // 結果がnullであることを検証
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_正常認証の場合_ユーザを返す()
        {
            // Arrange（準備）

            // 検証用にユーザを作成
            var user = new AppUserEntity
            {
                LoginCode = "test"
            };

            // パスワードをハッシュ化してユーザにセット
            var hasher = new PasswordHasher<AppUserEntity>();
            user.PasswordHash = hasher.HashPassword(
                user,
                "password"
            );

            // モックの戻り値を設定
            _mockRepository
                .Setup(x => x.GetByLoginCodeAsync("test"))
                .ReturnsAsync(user);

            // Act（実行）

            // 認証処理を実行
            var result = await _service.AuthenticateAsync(
                "test",
                "password"
            );

            // Assert（検証）

            // 結果がnullでないことを検証
            Assert.NotNull(result);

            // LoginCodeが正しいことを検証
            Assert.Equal(
                "test",
                result.LoginCode
            );

            // ハッシュが古くない前提として、再ハッシュ不要なのでUpdateされない
            // ※SuccessRehashNeeded分岐はPasswordHasher標準機能に依存してるので、成功前提でテストする
            _mockRepository.Verify(
                x => x.UpdateAsync(It.IsAny<AppUserEntity>()),
                Times.Never
            );
        }

        [Fact]
        public async Task AuthenticateAsync_GetByLoginCodeAsyncで例外発生時_例外を再throwする()
        {
            // Arrange（準備）

            // モックの戻り値を設定：GetByLoginCodeAsyncで例外が発生するように設定
            _mockRepository
                .Setup(x => x.GetByLoginCodeAsync("test"))
                .ThrowsAsync(new Exception("DBエラー"));

            // Act & Assert

            // 認証処理を実行して例外が発生することを検証
            await Assert.ThrowsAsync<Exception>(
                async () => await _service.AuthenticateAsync(
                    "test",
                    "password"
                )
            );

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