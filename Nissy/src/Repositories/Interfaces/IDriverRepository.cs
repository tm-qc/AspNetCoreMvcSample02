using Nissy.Models.Entity;

namespace Nissy.Repositories.Interfaces
{
    public interface IDriverRepository
    {
        IQueryable<DriverEntity> GetQuery();

        Task<DriverEntity?> FindAsync(int id);

        void Add(DriverEntity entity);

        void Update(DriverEntity entity);

        void Remove(DriverEntity entity);

        Task SaveChangesAsync();
    }
}