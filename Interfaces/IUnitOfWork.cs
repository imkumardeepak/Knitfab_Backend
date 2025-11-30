using AvyyanBackend.Models;
using AvyyanBackend.Models.ProAllot;

namespace AvyyanBackend.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Add repository properties here as you create specific repositories
        // Example: IProductRepository Products { get; }
        // Example: ICategoryRepository Categories { get; }
        // Example: ICustomerRepository Customers { get; }
        // Example: IOrderRepository Orders { get; }
        IRepository<TapeColorMaster> TapeColors { get; }
        IRepository<ShiftMaster> Shifts { get; }
        IRepository<StorageCapture> StorageCaptures { get; }
        IRepository<ProductionAllotment> ProductionAllotments { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}