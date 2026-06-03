using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Nissy.Attributes;
using Nissy.Constants;
using Nissy.Helpers;
using Nissy.Models;
using Nissy.Models.Entity;
using Nissy.Services.Interfaces;
using Nissy.ViewModels.Driver;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Nissy.Controllers
{
    //運転者マスタのコントローラー
    //※クラス名はモデルと合わせる

    //ルーティングはsnake_caseなので「Route 属性アノテーション」でコントローラーに個別設定とする
    [Route("ns_drivers")]
    public class DriverController : BaseController
    {
        //プロパティ
        private readonly IDriverService _driverService;
        private readonly ILogger<DriverController> _logger;


        //コンストラクタ
        public DriverController(
            MyContext context,
            ILogger<DriverController> logger,
            IDriverService driverService
        ) : base(context)// BaseController のコンストラクターに context を渡す
        {
            _logger = logger;
            _driverService = driverService;
        }


        // GET: Driver：一覧検索ページの表示
        [HttpGet("")]
        [PageTitle("運転者マスタ一覧")]
        [Breadcrumb(FromAction = "Index", FromController = "Top")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var viewModel = await _driverService.GetListAsync(page);
            return View(viewModel);
        }

        #region 未使用分 詳細ページの表示
        // GET: Driver/Details/5：詳細ページの表示
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var sample = await _context.Drivers
        //        .FirstOrDefaultAsync(m => m.id == id);
        //    if (sample == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(sample);
        //}
        #endregion

        // GET: Driver/Create：新規登録ページの表示
        [HttpGet("create")]//属性ルートの場合：アクションごとにルートを指定する必要がある
        [PageTitle("運転者マスタ 新規登録")]
        [Breadcrumb(FromAction = "Index", FromController = "Driver")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Driver/Create：新規登録処理
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //
        // これは単一行の受け取り。
        // 複数の場合はListなどで受け取る必要があるので、少し変わる↓
        // public async Task<IActionResult> CreateMultiple(List<Driver> drivers)
        [HttpPost("create")]
        [ValidateAntiForgeryToken]//CSRF対策
        public async Task<IActionResult> Create(CreateEditViewModel viewModel)
        {
            /*
             * TODO:不足分。後で考える
             * - カスタムバリデーションは必要になったら独自にアノテーション作成する予定(Attributeクラス作成)
             * - チェックとして必要なら「どういつユーザー」or「更新可能なロール」かなどいれる(ログインとか出来たときに)
             * - 複数テーブルの場合の切り戻しにロールバックの明示が必要。その時調べる
             */

            if (!ModelState.IsValid)
            {
                //バリエーションエラーの場合は、エラーを表示するために入力値を保持して登録ページに戻す
                return View(viewModel);
            }

            bool result = await _driverService.CreateAsync(viewModel);

            if (!result)
            {
                //TODO:失敗時にViewにアラート表示する
                return View(viewModel);//失敗時は編集ページに戻す。入力値も保持される。
            }

            return RedirectToAction(nameof(Index));// 成功→一覧へリダイレクト
        }


        // GET: Driver/Edit/{id}：編集ページの表示
        // ルートの {id} は必須。
        // id なしの /ns_drivers/edit/ は ASP.NET Core がルーティング段階で 404 を返す。
        [HttpGet("edit/{id}")]
        [PageTitle("運転者マスタ 編集")]
        [Breadcrumb(FromAction = "Index", FromController = "Driver")]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _driverService.GetEditViewModelAsync(id.Value);

            //データがない場合は404
            if (viewModel == null)
            {
                return NotFound();
            }

            ViewBag.DriverId = viewModel.Id; // formのrouteにidを渡す
            return View(viewModel); // ← DriverEditViewModelを渡す
        }

        // POST: Driver/Edit/{id}：編集処理
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEditViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var result = await _driverService.UpdateAsync(id, viewModel);

            if (!result)
            {
                //TODO:失敗時にViewにアラート表示する
                return View(viewModel);//失敗時は編集ページに戻す。入力値も保持される。
            }

            return RedirectToAction(nameof(Index)); // 成功時は一覧へ

        }

        #region 未使用分 削除ページの表示
        //// GET: Driver/Delete/5：削除ページの表示
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var sample = await _context.Drivers
        //        .FirstOrDefaultAsync(m => m.id == id);
        //    if (sample == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(sample);
        //}
        #endregion

        // POST: Driver/Delete/5：削除処理
        // ※DeleteConfirmed：この命名は同名、同引数の定義不可を回避するため、スキャフォールディングのデフォでこうなってる
        // ※ActionName：関数名がDeleteConfirmedだが、Action名をDeleteで呼べるようになる
        // ※現状ハーﾄﾞデリート
        // ※ソフトデリート（論理削除）にしたい場合：is_deleted などのフラグカラムを用意して Remove() の代わりに UPDATE する実装が必要
        // ※ソフトデリートの場合、取得ロジックでWhereで除外しないといけないが、MyContext にGlobal Query Filterを設定すれば全体で自動的に除外できるようになる
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _driverService.DeleteAsync(id);

            if (!result)
            {
                //TODO:失敗時にViewにアラート表示する
                TempData["ErrorMessage"] = Messages.Crud.DeleteTargetNotFound;
            }

            return RedirectToAction(nameof(Index));
        }

        #region 未使用分 存在確認
        //private bool DriverExists(int id)
        //{
        //    // 条件に一致するレコードが1件でも存在すれば true、なければ false
        //    return _context.Drivers.Any(e => e.Id == id);
        //}
        #endregion
    }
}
