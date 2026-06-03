namespace Nissy.Constants
{
    /// <summary>
    /// アプリ全体で使用するメッセージ定数
    /// </summary>
    public static class Messages
    {
        /// <summary>認証系メッセージ</summary>
        public static class Auth
        {
            public const string InvalidLoginCodeOrPassword = "ログインコードまたはパスワードが違います";
        }

        /// <summary>バリデーション系メッセージ</summary>
        public static class Validation
        {
            public const string LoginCodeRequired = "ログインコードは必須です";
            public const string PasswordRequired = "パスワードは必須です";
            public const string NameRequired = "氏名は必須です";
            public const string NameMaxLength = "氏名は30文字以内で入力してください";
        }


        /// <summary>CRUD系メッセージ</summary>
        public static class Crud
        {
            public const string DeleteTargetNotFound = "削除対象が存在しません";
            public const string UpdateTargetNotFound = "更新対象が存在しません";
            public const string CreateFailed = "登録に失敗しました";
        }
    }
}