using Doctor.Domain.Entities;
using System.Linq.Expressions;

namespace Doctor.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        // ============ Basic CRUD ============
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        // ============ Soft Delete ============
        Task SoftDeleteAsync(int id, string? deletedBy = null);
        Task RestoreAsync(int id);

        // ============ Query Helpers ============
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<IEnumerable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        // ============ Bulk Operations ============
        Task AddRangeAsync(IEnumerable<T> entities);
        void UpdateRange(IEnumerable<T> entities);
        void DeleteRange(IEnumerable<T> entities);

        // ============ Advanced Queries ============
        Task<IEnumerable<T>> GetWithIncludeAsync(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includeProperties);

        Task<T?> GetFirstWithIncludeAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties);

        // ============ Save ============
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Unit of Work interface for transaction management
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        bool HasActiveTransaction { get; }
    }
}
