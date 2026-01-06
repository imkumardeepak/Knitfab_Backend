using System.Collections.Generic;
using System.Threading.Tasks;
using AvyyanBackend.DTOs.Report;

namespace AvyyanBackend.Interfaces
{
    public interface IReportService
    {
        Task<List<FinalFabricReportDto>> GetFinalFabricReportAsync();
        Task<FinalFabricReportDto> GetFinalFabricReportBySalesOrderAsync(int salesOrderId);
    }
}