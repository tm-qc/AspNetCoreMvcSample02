using Nissy.Models.Entity;

namespace Nissy.ViewModels.Common
{
    public class PagerViewModel<T>
    {
        //IndexViewに受け渡しが必要なプロパティを定義
        //※ページャーは一覧ページのみの想定
        //※一覧以外のページは必要な時に考える

        //Pagerの基本情報
        public int TotalCount { get; set; } = 0;
        public int CurrentPage { get; set; } = 0;
        public int TotalPage { get; set; } = 0;
        public int DispStart { get; set; } = 0;
        public int DispEnd { get; set; } = 0;

        //表示される各ページの一覧データ
        public List<T> items { get; set; } = [];
    }
}
