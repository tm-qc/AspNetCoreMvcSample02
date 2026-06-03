using Microsoft.EntityFrameworkCore;
using Nissy.Controllers;
using Nissy.Helpers;
using Nissy.Models;
using Nissy.Models.Entity;
using Nissy.Repositories.Interfaces;
using Nissy.Services.Interfaces;
using Nissy.ViewModels.Driver;

namespace Nissy.Services
{
    /// <summary>
    /// 業務ロジックを記載するクラス
    /// ※データはリポジトリから取得して、業務ロジックを実装してデータを加工、コントローラーに返す
    /// </summary>
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ILogger<DriverService> _logger;

        public DriverService(
            IDriverRepository driverRepository, 
            ILogger<DriverService> logger
        )
        {
            _driverRepository = driverRepository;
            _logger = logger;
        }

        /// <summary>
        /// 一覧ページのデータを取得する
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IndexViewModel> GetListAsync(int page)
        {
            try {
                //大本の一覧データ取得
                var query = _driverRepository
                            .GetQuery()
                            .OrderBy(x => x.Id);

                // 表示データを取得（ページャー、一覧データ）
                var pager = await PagerHelper.CreateAsync(query, page, 5);

                // 型合わせ代入：表示のため一覧ページ用のIndexViewModelの型に変換する
                return new IndexViewModel
                {
                    TotalCount = pager.TotalCount,
                    CurrentPage = pager.CurrentPage,
                    TotalPage = pager.TotalPage,
                    DispStart = pager.DispStart,
                    DispEnd = pager.DispEnd,
                    items = pager.items,
                };
            }
            catch (Exception ex)
            {
                //TODO:catchの対応は一旦これで。Service層で拾って上層へ渡すイメージで
                _logger.LogError(ex, "エラー");
                // 上層へ渡す：この場では処理せずに呼び出し元のミドルウェアなどまで戻して、アプリ全体の例外処理ルールに任せ、Program.csに記載されてるエラー画面表示などさせるようにする
                throw;
            }
        }

        /// <summary>
        /// 新規登録の処理
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<bool> CreateAsync(CreateEditViewModel viewModel)
        {
            try
            {
                var entity = new DriverEntity
                {
                    Name = viewModel.Name,
                    BirthDate = viewModel.BirthDate,
                    RegistDate = viewModel.RegistDate,
                    Other = viewModel.Other,
                    LicenseNo = viewModel.LicenseNo,
                    ExpireDate = viewModel.ExpireDate,
                    LicenseDate = viewModel.LicenseDate,
                    LicenseKind = viewModel.LicenseKind,
                    LicenseCondition = viewModel.LicenseCondition,

                    ServiceTypeId = 1,
                    AccountManagerId = 1,
                    CreatedId = 1,
                    UpdatedId = 1,
                };

                _driverRepository.Add(entity);
                await _driverRepository.SaveChangesAsync();

                // 成功(失敗ならSaveChangesAsyncでExceptionになる)
                return true;
            }
            catch (Exception ex)
            {
                //TODO:catchの対応は一旦これで。Service層で拾って上層へ渡すイメージで
                _logger.LogError(ex, "エラー");
                // 上層へ渡す：この場では処理せずに呼び出し元のミドルウェアなどまで戻して、アプリ全体の例外処理ルールに任せ、Program.csに記載されてるエラー画面表示などさせるようにする
                throw;
            }
        }


        /// <summary>
        /// 編集ページ表示データ取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CreateEditViewModel?> GetEditViewModelAsync(int id)
        {
            try {
                var target = await _driverRepository.FindAsync(id);

                if (target == null)
                {
                    return null;
                }

                // 一括セットは仕様上できないので、必要な値をEntityからViewModelへ手動でセットする
                // Driver → DriverEditViewModel に手動で詰め替え
                // 任意データのnullの可能性警告：not nullが基本なのでありえないため ! で対応
                return new CreateEditViewModel
                {
                    Id = target.Id,
                    Name = target.Name,
                    BirthDate = target.BirthDate!,
                    RegistDate = target.RegistDate!,
                    Other = target.Other!,
                    LicenseNo = target.LicenseNo!,
                    ExpireDate = target.ExpireDate!,
                    LicenseDate = target.LicenseDate!,
                    LicenseKind = target.LicenseKind!,
                    LicenseCondition = target.LicenseCondition!,
                };
            }
            catch (Exception ex)
            {
                //TODO:catchの対応は一旦これで。Service層で拾って上層へ渡すイメージで
                _logger.LogError(ex, "エラー");
                // 上層へ渡す：この場では処理せずに呼び出し元のミドルウェアなどまで戻して、アプリ全体の例外処理ルールに任せ、Program.csに記載されてるエラー画面表示などさせるようにする
                throw;
            }
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(int id, CreateEditViewModel viewModel)
        {
            try
            {
                var target = await _driverRepository.FindAsync(id);

                // 対象データを取得し、存在しなければ失敗
                if (target == null)
                {
                    return false;
                }

                //TODO：ログイン完成後に必要なら対応。仕様に合わせてログインユーザ or 役割で更新権限あるか確認

                //取得したデータを基盤に更新が必要なので値を再セット
                //※仕様上更新データの追跡対象が重複するというエラーが起きる

                //値を一括セット
                // フォームの入力値を target に一括コピー
                // ※Bindしてないカラム=リクエスト受け取りしてない値がは初期化されるので結局使えない
                //_context.Entry(target).CurrentValues.SetValues(viewModel);

                //更新値をViewModelからDBのデータにセット
                // ViewModel → Entity に個別セット
                // ※SetValues は使わない（未バインドカラムが初期化されるリスクを避けるため）

                target.Name = viewModel.Name;
                //「?? string.Empty」について：「EF Core: UPDATE」の仕様上ここで「null」の時に「初期値に変換」しないとUPDATEでエラーになるので、
                //　　　　　　　　　　　　　　　カラムが「任意+not null+初期値」の場合は、セットの際に「Nullのときは初期値にする」が必要
                target.BirthDate = viewModel.BirthDate ?? string.Empty;
                target.RegistDate = viewModel.RegistDate ?? string.Empty;
                target.Other = viewModel.Other ?? string.Empty;
                target.LicenseNo = viewModel.LicenseNo ?? string.Empty;
                target.ExpireDate = viewModel.ExpireDate ?? string.Empty;
                target.LicenseDate = viewModel.LicenseDate ?? string.Empty;
                target.LicenseKind = viewModel.LicenseKind ?? string.Empty;
                target.LicenseCondition = viewModel.LicenseCondition ?? string.Empty;

                //TODO:送信値以外の値をログインできるまで暫定でセット
                target.ServiceTypeId = 1;
                target.AccountManagerId = 1;
                target.UpdatedId = 1;

                _driverRepository.Update(target);
                await _driverRepository.SaveChangesAsync();

                // 成功(失敗ならSaveChangesAsyncでExceptionになる)
                return true;
            }
            catch (Exception ex)
            {
                //TODO:catchの対応は一旦これで。Service層で拾って上層へ渡すイメージで
                _logger.LogError(ex, "エラー");
                // 上層へ渡す：この場では処理せずに呼び出し元のミドルウェアなどまで戻して、アプリ全体の例外処理ルールに任せ、Program.csに記載されてるエラー画面表示などさせるようにする
                throw;
            }

        }

        /// <summary>
        /// 削除処理
        /// </summary>
        /// <param name="id"></param>
        /// <returns>削除結果</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var target = await _driverRepository.FindAsync(id);

                if (target == null)
                {
                    return false;
                }

                _driverRepository.Remove(target);
                await _driverRepository.SaveChangesAsync();

                // 成功(失敗ならSaveChangesAsyncでExceptionになる)
                return true;

            }
            catch (Exception ex)
            {
                //TODO:catchの対応は一旦これで。Service層で拾って上層へ渡すイメージで
                _logger.LogError(ex, "エラー");
                // 上層へ渡す：この場では処理せずに呼び出し元のミドルウェアなどまで戻して、アプリ全体の例外処理ルールに任せ、Program.csに記載されてるエラー画面表示などさせるようにする
                throw;
            }
        }

    }
}