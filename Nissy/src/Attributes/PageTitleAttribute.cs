using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nissy.Attributes
{
    /// <summary>
    /// ページタイトルを設定するアノテーション
    /// 使用例：[PageTitle("一覧")]
    /// 
    /// ※「タイトルは必須」なので?なし
    /// ※ 名前付きではなく無名引数で受け取るのでsetなし、コンストラクタで受け取る形にする
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PageTitleAttribute : ActionFilterAttribute
    {
        public string Title { get; }

        public PageTitleAttribute(string title)
        {
            Title = title;
        }

        //アクション前にViewにページタイトルを渡す
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                controller.ViewData["Title"] = Title;
            }
            base.OnActionExecuting(context);
        }
    }
}