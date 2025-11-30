using Microsoft.EntityFrameworkCore.Storage;
using AvyyanBackend.Data;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using AvyyanBackend.Models.ProAllot;

namespace AvyyanBackend.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        
        // Add repository properties here as you create specific repositories
        private IRepository<TapeColorMaster>? _tapeColors;
        public IRepository<TapeColorMaster> TapeColors => _tapeColors ??= new Repository<TapeColorMaster>(_context);

        private IRepository<ShiftMaster>? _shifts;
        public IRepository<ShiftMaster> Shifts => _shifts ??= new Repository<ShiftMaster>(_context);

        private IRepository<StorageCapture>? _storageCaptures;
        public IRepository<StorageCapture> StorageCaptures => _storageCaptures ??= new Repository<StorageCapture>(_context);

        private IRepository<ProductionAllotment>? _productionAllotments;
        public IRepository<ProductionAllotment> ProductionAllotments => _productionAllotments ??= new Repository<ProductionAllotment>(_context);

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}