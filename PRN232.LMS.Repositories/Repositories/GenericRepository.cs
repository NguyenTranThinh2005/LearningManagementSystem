using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly Prn232LmsContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(Prn232LmsContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }
        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            string? orderBy = null,
            bool ascending = true,
            string[]? includes = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            IQueryable<T> query = _dbSet.AsQueryable();

            // Apply includes
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    if (!string.IsNullOrWhiteSpace(include))
                        query = query.Include(include.Trim());
                }
            }

            // Apply filter
            if (filter != null)
                query = query.Where(filter);

            // Get total count
            int totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                query = ApplyOrdering(query, orderBy, ascending);
            }

            // Apply paging
            int skip = (page - 1) * pageSize;
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        private static IQueryable<T> ApplyOrdering(IQueryable<T> source, string sortString, bool defaultAscending)
        {
            var sortExpressions = sortString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!sortExpressions.Any()) return source;

            IOrderedQueryable<T>? orderedQuery = null;

            for (int i = 0; i < sortExpressions.Length; i++)
            {
                var part = sortExpressions[i].Trim();
                bool ascending = defaultAscending;
                string propertyName = part;

                if (part.StartsWith("-"))
                {
                    ascending = false;
                    propertyName = part.Substring(1);
                }
                else if (part.StartsWith("+"))
                {
                    ascending = true;
                    propertyName = part.Substring(1);
                }

                try
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.Property(parameter, propertyName);
                    var lambda = Expression.Lambda(property, parameter);

                    string methodName;
                    if (i == 0)
                    {
                        methodName = ascending ? "OrderBy" : "OrderByDescending";
                    }
                    else
                    {
                        methodName = ascending ? "ThenBy" : "ThenByDescending";
                    }

                    var result = Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new Type[] { typeof(T), property.Type },
                        (orderedQuery ?? source).Expression,
                        Expression.Quote(lambda));

                    orderedQuery = (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(result);
                }
                catch
                {
                    // Skip invalid properties
                    continue;
                }
            }

            return orderedQuery ?? source;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }
    }
}
