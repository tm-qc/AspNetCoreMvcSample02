using Microsoft.EntityFrameworkCore;
using Nissy.Models;
using Nissy.Models.Entity;
using Nissy.Repositories.Interfaces;

namespace Nissy.Repositories
{
    /// <summary>
    /// リポジトリクラス
    /// ※基本的なDBアクセスを記載するクラス
    /// ※業務用へのデータの加工などはService層に記載する
    /// </summary>
    public class DriverRepository : IDriverRepository
    {
        private readonly MyContext _context;

        public DriverRepository(MyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 一覧取得用の基本クエリを返す
        /// </summary>
        /// <returns></returns>
        public IQueryable<DriverEntity> GetQuery()
        {
            // サービスでGetQuery()で基本クエリを取得して、調整するためのコンテキストだけ返す
            return _context.Drivers;
        }

        /// <summary>
        /// 対象データを一つ取得
        /// ※DBアクセスが発生するため非同期で実装
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DriverEntity?> FindAsync(int id)
        {
            return await _context.Drivers.FindAsync(id);
        }

        /// <summary>
        /// 新規登録データをセット
        /// ※メモリ上でのセットのみなので非同期不要
        /// ※DBへの保存はサービスでSaveChangesAsyncで行う
        /// </summary>
        /// <param name="entity"></param>
        public void Add(DriverEntity entity)
        {
            _context.Drivers.Add(entity);
        }

        /// <summary>
        /// 更新データをセット
        /// </summary>
        /// <param name="entity"></param>
        public void Update(DriverEntity entity)
        {
            _context.Drivers.Update(entity);
        }

        /// <summary>
        /// 削除データをセット
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(DriverEntity entity)
        {
            _context.Drivers.Remove(entity);
        }

        /// <summary>
        /// クエリ実行
        /// ※DBアクセスなので非同期で実装
        /// ※メソッドの実行はサービス層で行うが、すみわけのためにリポジトリクラスで実装する
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}