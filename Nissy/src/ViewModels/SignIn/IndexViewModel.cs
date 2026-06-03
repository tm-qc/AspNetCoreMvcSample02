using Nissy.Models.Entity;
using Nissy.ViewModels.Common;
using Nissy.Constants;
using System.ComponentModel.DataAnnotations;

namespace Nissy.ViewModels.SignIn
{
    // 「一覧データ+ページャーのデータ」は共通のPaginationViewModelで定義しているのでそれを継承して利用する
    // 一覧ページはIndexViewModelを参照している（型合わせのため、コントローラでPaginationViewModel→IndexViewModelに再格納してる）
    // ※SignInについてはデータ取得なし+ID、PW送信なので「PagerViewModel<AppUserEntity>」継承不要で進める
    public class IndexViewModel
    {
        [Required(ErrorMessage = Messages.Validation.LoginCodeRequired)]
        public string LoginCode { get; set; } = string.Empty;

        [Required(ErrorMessage = Messages.Validation.PasswordRequired)]
        public string LoginPw { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}