using AvyyanBackend.DTOs.DispatchPlanning;
using AvyyanBackend.Models;

namespace AvyyanBackend.Interfaces
{
    public interface IDispatchPlanningRepository
    {
        Task<IEnumerable<DispatchPlanning>> GetAllAsync();
        Task<DispatchPlanning?> GetByIdAsync(int id);
        Task<DispatchPlanning?> GetByLotNoAsync(string lotNo);
        Task<DispatchPlanning> CreateAsync(DispatchPlanning dispatchPlanning);
        Task<DispatchPlanning> UpdateAsync(int id, DispatchPlanning dispatchPlanning);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<DispatchedRoll>> GetDispatchedRollsByPlanningIdAsync(int planningId);
        Task<DispatchedRoll> CreateDispatchedRollAsync(DispatchedRoll dispatchedRoll);
        Task<IEnumerable<DispatchedRoll>> CreateDispatchedRollsBulkAsync(IEnumerable<DispatchedRoll> dispatchedRolls);
        Task<string> GenerateLoadingNoAsync();
        Task<string> GenerateDispatchOrderIdAsync();
    }
}