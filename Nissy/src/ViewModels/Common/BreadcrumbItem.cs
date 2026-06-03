namespace Nissy.ViewModels.Common
{
    /// <summary>パンくず1件分のデータ</summary>
    public class BreadcrumbItem
    {
        /// <summary>表示名</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>リンクURL（現在ページはnull）</summary>
        public string? Url { get; set; }
    }
}