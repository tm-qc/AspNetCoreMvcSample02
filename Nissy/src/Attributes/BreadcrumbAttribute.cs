namespace Nissy.Attributes
{
    /// <summary>
    /// パンくずを設定するアノテーション
    /// 使用例：[Breadcrumb(FromAction = "Index", FromController = "Driver")]
    /// ※ページタイトルはPageTitleAttributeで設定する
    /// ※TOPページはfromなしで[Breadcrumb()]とする
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BreadcrumbAttribute : Attribute
    {
        // <summary>遷移元アクション名</summary>
        public string? FromAction { get; set; }

        // <summary>遷移元コントローラー名（省略時は同じコントローラー）</summary>
        public string? FromController { get; set; }
    }
}