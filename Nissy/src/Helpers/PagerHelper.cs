using Microsoft.EntityFrameworkCore;
using Nissy.Models.Entity;
using Nissy.ViewModels.Common;

namespace Nissy.Helpers
{

    // ツール全体の共通処理はHelperクラスで記載
    public class PagerHelper
    {
        /// <summary>
        /// ページャーの関数
        /// 現在のページに合わせたページャー + 一覧の表示データを取得
        /// ※呼び出しやすいようにstatic関数にしてる
        /// </summary>
        /// 
        /// <typeparam name="T">参照元のEntity</typeparam>
        /// <param name="query">参集元のデータ</param>
        /// <param name="page">現在のページ</param>
        /// <param name="pageSize">1ページあたりのデータの表示量</param>
        /// <returns>ページャー情報 + 表示データ</returns>
        public static async Task<PagerViewModel<T>> CreateAsync<T>(
            IQueryable<T> query,
            int page,
            int pageSize)
        {
            // ページの現在位置を算出
            // ※ページ番号(1始まり) → Skipのオフセット(0始まり) に変換（page - 1）
            int pageNum = page - 1;

            // 全表示データ数をカウント
            int totalCount = await query.CountAsync();

            // 現在のページに表示するデータを取得
            List<T> items = await query.Skip(pageSize * pageNum).Take(pageSize).ToListAsync();

            // 表示件数の開始・終了を計算
            // 最終ページは端数になるため、Math.MinでtotalCountと比較して小さい方を使う
            int dispStart = (page - 1) * pageSize + 1;
            int dispEnd = Math.Min(page * pageSize, totalCount);

            // 総ページ数を計算
            // Math.Ceilingで小数点以下を切り上げ
            // 例えば10件データがある場合は10/3=3.3333333333333335となり、この場合4ページとなるため
            int totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var viewModel = new PagerViewModel<T>
            {
                TotalCount = totalCount,
                CurrentPage = page,
                TotalPage = totalPage,
                DispStart = dispStart,
                DispEnd = dispEnd,
                items = items,
            };

            return viewModel;
        }
    }
}
