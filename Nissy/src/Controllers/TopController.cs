using Microsoft.AspNetCore.Mvc;
using Nissy.Attributes;
using Nissy.Models;

namespace Nissy.Controllers
{
    //ルーティングはsnake_caseなので「Route 属性アノテーション」でコントローラーに個別設定とする
    [Route("/top")]
    public class TopController : BaseController
    {
        public TopController(MyContext context) : base(context)
        {
        }

        //HTTP属性は明示、固定の観点で記載するルールにする。
        //※記載なし出も動くが、デフォ状態ですべて受け付ける状態
        //※HTTP属性は同じURLでアクセス分けたいときは必須

        // GET: パンくずリンクや直接アクセス用
        [HttpGet("")]
        [PageTitle("マスタ管理")]
        [Breadcrumb()]
        public IActionResult Index()
        {
            return View();
        }

        // POST: ログインフォームからの送信用
        [HttpPost("")]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return View();
        }
    }
}
