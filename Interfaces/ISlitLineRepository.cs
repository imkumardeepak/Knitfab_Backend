using AvyyanBackend.Models;

namespace AvyyanBackend.Interfaces
{
    public interface ISlitLineRepository
    {
        Task<IEnumerable<SlitLineMaster>> GetAllAsync();
        Task<SlitLineMaster?> GetByIdAsync(int id);
        Task<SlitLineMaster> CreateAsync(SlitLineMaster slitLine);
        Task<SlitLineMaster?> UpdateAsync(SlitLineMaster slitLine);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<SlitLineMaster>> SearchAsync(string? slitLine, char? slitLineCode, bool? isActive);
    }
}