using Microsoft.AspNetCore.Identity;
using Nissy.Controllers;
using Nissy.Models.Entity;
using Nissy.Services.Interfaces;

namespace Nissy.Services;

public class AuthService : IAuthService
{
    private readonly IAppUserRepository _appUserRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAppUserRepository appUserRepository,
        ILogger<AuthService> logger
        )
    {
        _appUserRepository = appUserRepository;
        _logger = logger;
    }

    /// <summary>
    /// ログイン認証
    /// </summary>
    /// <param name="loginCode"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<AppUserEntity?> AuthenticateAsync(string loginCode, string password)
    {
        try
        {
            var user = await _appUserRepository.GetByLoginCodeAsync(loginCode);

            // ユーザが存在しない場合はnullを返す
            if (user == null)
            {
                return null;
            }

            // 万が一PasswordHasherを持っていない場合はログイン画面にも戻す
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return null;
            }

            // パスワードチェック
            var hasher = new PasswordHasher<AppUserEntity>();
            var verifyResult = hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            );

            //認証失敗の場合nullを返す
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // 古いハッシュ方式ならPassworHashを更新
            // NETのバージョンが上がったり、設定が変わったりしたときに作り直した方が安全という場合に「PasswordVerificationResult.SuccessRehashNeeded」が返る
            if (verifyResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = hasher.HashPassword(user, password);
                await _appUserRepository.UpdateAsync(user);
            }

            return user;
        }
        catch (Exception ex)
        {
            //TODO:catchの対応は一旦これで。Service層で拾って上層へ渡すイメージで
            _logger.LogError(ex, "エラー");
            // 上層へ渡す：この場では処理せずに呼び出し元のミドルウェアなどまで戻して、アプリ全体の例外処理ルールに任せ、Program.csに記載されてるエラー画面表示などさせるようにする
            throw;
        }


    }
}