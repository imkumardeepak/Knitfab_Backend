using AvyyanBackend.DTOs.SalesOrder;

namespace AvyyanBackend.Interfaces
{
	public interface ISalesOrderService
	{
		Task<IEnumerable<SalesOrderResponseDto>> GetAllSalesOrdersAsync();

		Task<IEnumerable<SalesOrderResponseDto>> GetAllSalesOrdersByProcessFlagAsync(int processFlag);
		Task<SalesOrderResponseDto?> GetSalesOrderByIdAsync(int id);
		Task<SalesOrderResponseDto> CreateSalesOrderAsync(CreateSalesOrderRequestDto createSalesOrderDto);
		Task<SalesOrderResponseDto?> UpdateSalesOrderAsync(int id, UpdateSalesOrderRequestDto updateSalesOrderDto);
		Task<bool> DeleteSalesOrderAsync(int id);
		Task<IEnumerable<SalesOrderResponseDto>> SearchSalesOrdersAsync(SalesOrderSearchRequestDto searchDto);
		Task<bool> IsVoucherNumberUniqueAsync(string voucherNumber, int? excludeId = null);
		Task<IEnumerable<SalesOrderResponseDto>> GetUnprocessedSalesOrdersAsync();
		Task<bool> MarkAsProcessedAsync(int id);
		Task<bool> MarkSalesOrderItemAsProcessedAsync(int salesOrderId, int salesOrderItemId);
	}
}