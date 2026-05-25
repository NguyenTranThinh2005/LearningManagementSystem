using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Get all entities
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Get entity by id
        /// </summary>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Get entities by condition
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Get paginated entities with sorting, filtering, and inclusion
        /// </summary>
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            string? orderBy = null,
            bool ascending = true,
            string[]? includes = null);

        /// <summary>
        /// Add new entity
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Add multiple entities
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Delete multiple entities
        /// </summary>
        void DeleteRange(IEnumerable<T> entities);

        /// <summary>
        /// Save changes to database
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Check if entity exists
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
    }
}
