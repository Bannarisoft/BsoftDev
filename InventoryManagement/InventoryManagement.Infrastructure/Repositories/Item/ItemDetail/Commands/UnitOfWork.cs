using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _db;
        private IDbContextTransaction? _tx;

        public UnitOfWork(ApplicationDbContext db) => _db = db;

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _tx = await _db.Database.BeginTransactionAsync(ct);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_tx != null) await _tx.CommitAsync(ct);
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_tx != null) await _tx.RollbackAsync(ct);
        }

        public void Dispose()
        {
            _tx?.Dispose();
        }
    }
}
