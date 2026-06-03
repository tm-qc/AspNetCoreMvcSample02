using Nissy.Models.Entity;
using Nissy.ViewModels.Common;

namespace Nissy.ViewModels.Driver
{
    // 「一覧データ+ページャーのデータ」は共通のPaginationViewModelで定義しているのでそれを継承して利用する
    // 一覧ページはIndexViewModelを参照している（型合わせのため、コントローラでPaginationViewModel→IndexViewModelに再格納してる）
    public class IndexViewModel:PagerViewModel<DriverEntity>
    {
        //ここに記載するものの想定
        //
        // - 「一覧データ+ページャーのデータ」以外で一覧ページで必要なプロパティ
        // - 個別にバリデーションなどのアノテーションが必要になったプロパティ
        //
        // などがあればここに定義して調整する予定
        // 基本は「一覧データ+ページャーのデータ」で行けるはず
    }
}