using Doctor.Domain.Interfaces;
using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Doctor.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DoctorDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public UnitOfWork(DoctorDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                var result = await _context.SaveChangesAsync();
                _logger.LogInformation($"Saved {result} changes to database");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes");
                throw;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes");
                throw;
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("A transaction is already in progress");

            _currentTransaction = await _context.Database.BeginTransactionAsync();
            _logger.LogInformation("Transaction started");
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction in progress");

            try
            {
                await SaveChangesAsync();
                await _currentTransaction.CommitAsync();
                _logger.LogInformation("Transaction committed");
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction in progress");

            try
            {
                await _currentTransaction.RollbackAsync();
                _logger.LogInformation("Transaction rolled back");
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}