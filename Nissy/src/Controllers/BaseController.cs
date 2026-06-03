using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Models.Entity;
using Nissy.ViewModels.Driver;

namespace Nissy.Controllers
{
    //ツール全体の共通処理を行うコントローラー
    public class BaseController : Controller
    {
        protected readonly MyContext _context;

        public BaseController(MyContext context)
        {
            _context = context;
        }

        // 全アクション実行前に行う共通処理
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

             // コントローラーアクション前の全体共通の処理

        }
    }
}