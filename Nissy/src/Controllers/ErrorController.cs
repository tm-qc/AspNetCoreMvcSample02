using Microsoft.AspNetCore.Mvc;

namespace Nissy.Controllers
{
    //TODO:暫定:ログイン出来てどこに戻すべきか？決めた方がいいかも
    [Route("error")]
    public class ErrorController : Controller
    {
        [HttpGet("{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;
            return View();
        }
    }
}