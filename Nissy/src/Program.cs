using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Repositories;
using Nissy.Repositories.Interfaces;
using Nissy.Seeders;
using Nissy.Services;
using Nissy.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // CSRFグローバル設定
    // すべてのPOST/PUT/PATCH/DELETEリクエストに対してCSRFトークン検証をグローバルに適用
    // <form> に asp-action と asp-controller を追加が必須
    //
    // ※HTTPのGET HEAD OPTIONS TRACEにはトークン検証しない（あまり普段必要なこと少ない）
    // ※局所的に無効化が必要な場合は[IgnoreAntiForgeryToken]アノテーション付与する（基本ない）
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());

    // 認証グローバル設定（全ページログイン必須）
    // ※ログイン不要なページはアノテーション：[AllowAnonymous]を設定する
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

    options.Filters.Add(new AuthorizeFilter(policy));

    // string（非null）プロパティへの暗黙的な [Required] を無効化
    // ※ Nullable enable 環境(C# 8.0 以降の機能で、参照型（string など）に null が入るかどうかをコンパイラが管理する設定)
    //    では string の?なしは非null扱いになり自動で必須バリデーションがかかるためそれを無効化。
    //    必須は自分でアノテーションで [Required] をつける
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});


// Cookie認証を有効化
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/sign_in";   // 未ログイン時の遷移先
        options.LogoutPath = "/sign_in/logout";

        //JavaScriptからCookieを読めないようにする設定
        options.Cookie.HttpOnly = true;
        //他サイト経由のリクエストでCookieを送るかどうか の制御
        //※Lax:通常は送らないが、リンク遷移などはOK（一般的）
        options.Cookie.SameSite = SameSiteMode.Lax;
        //HTTPS通信のときだけCookieを送るか の設定
        //Always：HTTPSじゃないとCookie送らない
        //SameAsRequest：HTTPSならSecure付ける、HTTPなら付けない
        //None：HTTPでもSecureを付けない
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);//認証Cookieの有効期限(分)
        options.SlidingExpiration = true;//ユーザーが操作している間は、期限が延長されるかどうか
    });

builder.Services.AddAuthorization();

// DB接続のためのモデルコンテキストを登録
// 接続文字列は appsettings.json の "ConnectionStrings" セクションの "DefaultConnection" を取得
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );

    // 開発環境のみ：実際のパラメータ値をログに出力する
    // ※開発環境以外ではセキュリティ上の理由で無効化することが推奨される(ログにログイン情報などが残る)
    // ※launchSettings.jsonの "ASPNETCORE_ENVIRONMENT": "Development" で環境変数を設定しているため、開発環境ではこのオプションが有効になる
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }

});

// サービスクラスの登録
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDriverService, DriverService>();

// リポジトリクラスの登録
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();

var app = builder.Build();

// DIコンテナ(機能させるためのクラス)の注入準備でscope作成
using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    // ServiceProviderはこんなものを主に呼び出せ、起動時に設定などできる
    // 
    //  一般的によく使われるもの
    // 
    //  データベースコンテキスト (MyContext)
    //  ロガー (ILogger)
    //  認証関連 (UserManager)
    //  カスタムサービス (独自のビジネスロジック)
    //  設定情報 (IConfiguration)

    // scopeにServiceProvider(アプリ起動時に動く機能)を注入
    IServiceProvider provider = scope.ServiceProvider;

    // Seederでデータを投入するために追加
    // 〇〇Seed.csのInitializeメソッドにprovider(ServiceProvider)を渡して必要なサービスを呼び出せるようにする
    // Initializeがasyncもってるのでawait追加が必要
    await InitDataSeeder.Initialize(provider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //TODO:本番機のみにデフォでなってる+規約ルートになってるので現状はつながらない。
    //あとでエラー遷移決めるときに例外（クラッシュ）エラーのときはどうするかきめる

    //デフォルトであるコードで例外（クラッシュ）用とのこと。
    //(try-catch で捕まえられなかった例外、サーバー内部で落ちた系のエラー（500）など)
    app.UseExceptionHandler("/Home/Error");
                                           
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//TODO:暫定:ログイン出来てどこにどう戻すべきか？決めた方がいいかも
// Errorページ：404などレスポンスの StatusCode が 400～599の場合に全環境で専用のエラーページに誘導する
//
// - 4xxクライアント側の問題（リクエストが悪い）404 Not Found、403 Forbidden、401 Unauthorized
// - 5xxサーバー側の問題（サーバーが失敗した）500 Internal Server Error、503 Service Unavailable
//   ※500は例外側が担当することが多い
//など
//
// HTTPステータスコード用のエラーページ。
// {0}はステータスコードが入る。例：/error/404
// Routingより前 に書く必要がある
app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// TODO:順番重要??なぜ？
app.UseAuthentication();
app.UseAuthorization();

// 属性ルートを使うコントローラーが混在するため、規約ルートは使用しない。混在してつかわない
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

// 属性ルートでルーティング管理とするため MapControllers() に変更
// ※規約ルートと違ってアクション=コントローラのメソッド名は自動で認識しない。そのため[HttpGet("create")]のようにアクションごとにルートを指定する必要がある
app.MapControllers();

// デフォルトページ：「/」へのアクセスを /sign_in にリダイレクト
app.MapGet("/", context =>
{
    //認証済みの場合はTOPへ
    if (context.User.Identity?.IsAuthenticated == true)
    {
        context.Response.Redirect("/top");
    }
    //未認証ならログイン画面へ
    else
    {
        context.Response.Redirect("/sign_in");
    }

    return Task.CompletedTask;
});

app.Run();
