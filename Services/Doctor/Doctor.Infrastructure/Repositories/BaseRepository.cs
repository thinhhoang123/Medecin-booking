using Doctor.Domain.Entities;
using System.Linq.Expressions;
using Doctor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Doctor.Infrastructure.Repositories
{
    /// <summary>
    /// Generic base repository implementation with common CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger<BaseRepository<T>> _logger;

        protected BaseRepository(DbContext context, ILogger<BaseRepository<T>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public virtual async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Where(e => !e.IsDeleted)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entity by id {Id}", id);
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet
                    .Where(e => !e.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all entities");
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet
                    .Where(predicate)
                    .Where(e => !e.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding entities with predicate");
                throw;
            }
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                entity.SetCreatedInfo();
                await _dbSet.AddAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding entity");
                throw;
            }
        }

        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                entity.SetUpdatedInfo();
                _dbSet.Update(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity");
                throw;
            }
        }

        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                _dbSet.Remove(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity");
                throw;
            }
        }


        public virtual async Task SoftDeleteAsync(int id, string? deletedBy = null)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    entity.SoftDelete(deletedBy);
                    Update(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting entity with id {Id}", id);
                throw;
            }
        }

        public virtual async Task RestoreAsync(int id)
        {
            try
            {
                var entity = await _dbSet
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted);

                if (entity != null)
                {
                    entity.Restore();
                    Update(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring entity with id {Id}", id);
                throw;
            }
        }

        // ============ Query Helpers ============

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet
                    .Where(e => !e.IsDeleted)
                    .AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if entity exists");
                throw;
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            try
            {
                var query = _dbSet.Where(e => !e.IsDeleted);
                if (predicate != null)
                    query = query.Where(predicate);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting entities");
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;

                var query = _dbSet.Where(e => !e.IsDeleted);

                if (predicate != null)
                    query = query.Where(predicate);

                if (orderBy != null)
                    query = orderBy(query);

                return await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged entities");
                throw;
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                {
                    entity.SetCreatedInfo();
                }

                await _dbSet.AddRangeAsync(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding range of entities");
                throw;
            }
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                {
                    entity.SetUpdatedInfo();
                }

                _dbSet.UpdateRange(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating range of entities");
                throw;
            }
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                _dbSet.RemoveRange(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range of entities");
                throw;
            }
        }

        // ============ Advanced Queries ============

        public virtual async Task<IEnumerable<T>> GetWithIncludeAsync(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                var query = _dbSet.Where(e => !e.IsDeleted);

                if (predicate != null)
                    query = query.Where(predicate);

                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entities with includes");
                throw;
            }
        }

        public virtual async Task<T?> GetFirstWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                var query = _dbSet.Where(e => !e.IsDeleted);

                query = query.Where(predicate);

                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting first entity with includes");
                throw;
            }
        }


        public virtual async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while saving changes");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving changes");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes");
                throw;
            }
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while saving changes");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving changes");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes");
                throw;
            }
        }

        // ============ Utility Methods ============

        protected IQueryable<T> GetQueryable()
        {
            return _dbSet.Where(e => !e.IsDeleted);
        }

        protected IQueryable<T> GetQueryableWithDeleted()
        {
            return _dbSet.IgnoreQueryFilters();
        }

        protected async Task<bool> SaveChangesWithRetryAsync(int maxRetries = 3)
        {
            var retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    await SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    retryCount++;
                    if (retryCount == maxRetries)
                        throw;
                }
            }
            return false;
        }
    }
}
