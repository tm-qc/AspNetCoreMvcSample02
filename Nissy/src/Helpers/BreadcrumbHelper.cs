using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nissy.Attributes;
using Nissy.Models;
using Nissy.ViewModels.Common;
using System.Reflection;

namespace Nissy.Helpers
{
    public static class BreadcrumbHelper
    {
        /// <summary>
        /// パンくずを再帰的に組み立てる
        /// 
        /// 【目的】
        /// 現在実行中の Action に付いている BreadcrumbAttribute を取得し、
        /// アノテーションに設定されてる遷移元の FromAction / FromController を辿って親ページを再帰的に追いかけ、
        /// パンくずリスト（親→子）を作成する。
        /// </summary>
        /// <param name="httpContext">現在のリクエスト情報（URL、ルーティング情報などが入っている）</param>
        /// <returns>パンくずリスト（親→子の順番）</returns>
        public static List<BreadcrumbItem> BuildBreadcrumb(HttpContext httpContext)
        {
            //EndPoint：クリックした後の実行中(遷移先)の情報を取得
            var endpoint = httpContext.GetEndpoint();
            //情報がない場合：MVCのController/Actionに該当しないリクエストなので空をかえす
            if (endpoint == null) return new List<BreadcrumbItem>();

            //GetMetadata<ControllerActionDescriptor>：実行中(遷移先)の「Controller/Action」の情報を取得
            var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            //情報がない場合：MVCのController/Actionに該当しないリクエストなので空をかえす
            if (actionDescriptor == null) return new List<BreadcrumbItem>();

            // パンくずを格納するリスト
            var items = new List<BreadcrumbItem>();

            //「実行中(遷移先)のリクエスト情報」から取得した実行中(遷移先)のコントローラ名
            var currentController = actionDescriptor.ControllerName;
            //「実行中(遷移先)のリクエスト情報」から取得した実行中(遷移先)のアクション名
            var currentAction = actionDescriptor.ActionName;

            // パンくず情報を生成する
            // ※再帰的に辿る
            BuildRecursive(
                items,
                actionDescriptor.ControllerTypeInfo.AsType(),
                currentController,
                currentAction,
                httpContext
            );

            // 逆順にする（「子→親」を「親→子」の順にする）
            items.Reverse();

            return items;
        }

        /// <summary>
        /// パンくず情報を生成する
        /// 
        /// 指定されたController/Actionの BreadcrumbAttribute を読み取り、
        /// 親があればさらに辿ってパンくず情報を items に追加していく。
        /// 
        /// ※再帰処理用の関数
        /// </summary>
        /// 
        /// <param name="items">パンくず情報の格納先</param>
        /// <param name="controllerType">アノテーション情報を取得するために必要なControllerのType情報</param>
        /// <param name="controllerName">実行中(遷移先)のコントローラ名</param>
        /// <param name="actionName">実行中(遷移先)のアクション名</param>
        private static void BuildRecursive(
            List<BreadcrumbItem> items,
            Type controllerType,
            string controllerName,
            string actionName,
            HttpContext httpContext
        )
        {
            // 「指定されたAction名のメソッド」をControllerクラスから探して取得する
            //　メソッドに付いている [Breadcrumb] Attribute を参照するために必要
            // GetMethod(name) はオーバーロード(GET/POSTの二つとか)があると AmbiguousMatchException になるため、
            // GetMethods() で全取得し BreadcrumbAttribute が付いているものを選ぶ
            var method = controllerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == actionName)
                .FirstOrDefault(m => m.GetCustomAttribute<BreadcrumbAttribute>() != null);
            // 指定したAction名のメソッドが見つからないなら終了
            if (method == null) return;

            // PageTitleAttribute を取得
            var pageTitleAttr = method.GetCustomAttribute<PageTitleAttribute>();

            // PageTitle未設定対策：開発中はエラー、本番機はエラーなしで No PageTitle とわかるようにする
            // ※Viewに渡す機能を実装したタイミングで表示ページにPageTitleのアノテーションがないとエラーになるようにはなった
            #if DEBUG
            if (pageTitleAttr == null)
                {
                    throw new InvalidOperationException(
                        $"[PageTitle] が未設定です" +
                        $"Controller: {controllerName}, Action: {actionName} に [PageTitle(\"タイトル名\")] を追加してください。"
                    );
                }
            #endif

            // メソッドに付いているアノテーション BreadcrumbAttribute の設定値を取得する。
            var attr = method.GetCustomAttribute<BreadcrumbAttribute>();
            // Attributeが付いていないならパンくず対象外なので終了
            if (attr == null) return;

            // LinkGeneratorは公式で、ルーティング対応のURL生成をするための標準機能
            // GetRequiredService：DIコンテナからサービスを取得できる
            // ※Viewから渡す代わりにHelper内で直接取得する
            var linkGenerator = httpContext.RequestServices.GetRequiredService<LinkGenerator>();

            // LinkGeneratorを使って「ルーティングに従った正しいURL」を生成する
            // （[Route], [HttpGet("")], Areaなどを考慮してくれる）
            var url = linkGenerator.GetPathByAction(
                httpContext,
                action: actionName,
                controller: controllerName
            );

            //パンくずに設定するタイトル、URLをセット
            items.Add(new BreadcrumbItem
            {
                // null許容：無いときはPageTitleの実装漏れなのでNo PageTitleとわかるようにする
                Title = pageTitleAttr?.Title ?? "No PageTitle",
                Url = url ?? "#"
            });

            // 親が設定されているなら辿る
            if (!string.IsNullOrEmpty(attr.FromController) && !string.IsNullOrEmpty(attr.FromAction))
            {
                // 親ControllerのNameを取得する（簡易版）
                // attr.FromController が "Products" の場合
                // "Nissy.Controllers.ProductsController" のようなクラス名を組み立てる。
                //　controllerType.Namespace を使う理由：現在のControllerと同じnamespaceにいる前提で探すため
                //　　　　　　　　　　　　　　　　　　　　→これは今はController配下にフォルダ内からなりたってるだけ
                var parentControllerTypeName = controllerType.Namespace + "." + attr.FromController + "Controller";

                // 親ControllerのTypeを取得する
                var parentControllerType = controllerType.Assembly.GetType(parentControllerTypeName);

                // 親Controllerが見つかったら再帰で辿る
                if (parentControllerType != null)
                {
                    BuildRecursive(
                        items,
                        parentControllerType,
                        attr.FromController,
                        attr.FromAction,
                        httpContext
                    );
                }
            }
        }
    }
}