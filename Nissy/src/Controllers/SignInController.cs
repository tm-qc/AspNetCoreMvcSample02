using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nissy.Attributes;
using Nissy.Constants;
using Nissy.Models;
using Nissy.Services.Interfaces;
using Nissy.ViewModels.SignIn;
using System.Security.Claims;

namespace Nissy.Controllers
{

    // ルーティングはsnake_caseなので「Route 属性アノテーション」でコントローラーに個別設定とする
    // 本当はSignInじゃなくAuthとかLoginが良さそうだが、今回の資料に合わせる
    [Route("sign_in")]
    // サインインページはログイン不要
    [AllowAnonymous]
    public class SignInController : BaseController
    {
        private readonly IAuthService _authService;

        //public SignInController(MyContext context) : base(context){}
        public SignInController(MyContext context,IAuthService authService) : base(context)
        {
            _authService = authService;
        }

        //HTTP属性は明示、固定の観点で記載するルールにする。
        //※記載なしでも動くが、デフォ状態ですべて受け付ける状態
        //※HTTP属性は同じURLでアクセス分けたいときは必須

        //GET:ページ表示
        [HttpGet("")]
        [PageTitle("ログイン")]
        public IActionResult Index()
        {
            //ログイン済みの場合Topに遷移させる
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Top");
            }

            return View();
        }

        // POST: ログインフォームからの送信用
        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ログイン認証
            var user = await _authService.AuthenticateAsync(
                model.LoginCode,
                model.LoginPw
            );

            // 認証失敗ならログイン画面に戻す
            if (user == null)
            {
                ModelState.AddModelError("", Messages.Auth.InvalidLoginCodeOrPassword);
                return View(model);
            }

            // ServiceはEntityだけ返してControllerがClaimsを組むようにすみ分け
            // ※コントローラーでClaimsを組む理由：業務ロジックではなくClaimがフレームワークのWeb認証設定のためコントローラーに置く
            // ※置き方は現場によって色々あるので、現場の文化に依存することもある

            // Claims作成（ログイン情報を暗号化し保持）
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("LoginCode", user.LoginCode),
            };

            // claimsをCookie認証用と設定する
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Cookieを発行してログイン状態にする(.AspNetCore.Cookiesがブラウザにできる)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                RememberMe(model.RememberMe)
            );

            return RedirectToAction("Index", "Top");
        }

        // Post: /Logout
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //認証Cookieを無効化 / 削除
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "SignIn");
        }


        ///Private

        /// <summary>
        /// ログイン状態の保持
        /// ※AuthenticationProperties：業務ロジックではなくフレームワークのWeb認証設定のためコントローラーに置く
        /// ※置き方は現場によって色々あるので、現場の文化に依存することもある
        /// </summary>
        /// <param name="rememberMe">ログイン状態の保持のチェックボックスの値</param>
        /// <returns>AuthenticationProperties：認証系の設定オブジェクト</returns>
        private AuthenticationProperties RememberMe(bool rememberMe = false)
        {
            //AuthenticationProperties は Microsoft.AspNetCore.Authentication に入ってる標準クラス
            var authProperties = new AuthenticationProperties
            {
                //永続Cookie（ブラウザ閉じても残る）
                IsPersistent = rememberMe,
            };

            // チェックONの場合のみ有効期限を長くする
            if (rememberMe)
            {
                //30日間ログイン状態を保持する
                //※Program.csの「TimeSpan.FromMinutes(30)」とは別に個別の期限設定となる
                authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30);
            }

            return authProperties;
        }

    }
}
