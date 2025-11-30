using AvyyanBackend.Models;

namespace AvyyanBackend.Interfaces
{
    public interface ISalesOrderWebRepository
    {
        Task<IEnumerable<SalesOrderWeb>> GetAllAsync();
        Task<SalesOrderWeb?> GetByIdAsync(int id);
        Task<SalesOrderWeb> CreateAsync(SalesOrderWeb salesOrderWeb);
        Task<SalesOrderWeb?> UpdateAsync(int id, SalesOrderWeb salesOrderWeb);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}