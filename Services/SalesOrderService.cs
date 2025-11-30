using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.SalesOrder;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
	public class SalesOrderService : ISalesOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRepository<SalesOrder> _salesOrderRepository;
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<SalesOrderService> _logger;

		public SalesOrderService(
			IUnitOfWork unitOfWork,
			IRepository<SalesOrder> salesOrderRepository,
			ApplicationDbContext context,
			IMapper mapper,
			ILogger<SalesOrderService> logger)
		{
			_unitOfWork = unitOfWork;
			_salesOrderRepository = salesOrderRepository;
			_context = context;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<IEnumerable<SalesOrderResponseDto>> GetAllSalesOrdersAsync()
		{
			_logger.LogDebug("Getting all sales orders");
			var salesOrders = await _context.SalesOrders
				.Include(so => so.Items)
				.ToListAsync();
			_logger.LogInformation("Retrieved {SalesOrderCount} sales orders", salesOrders.Count());
			return _mapper.Map<IEnumerable<SalesOrderResponseDto>>(salesOrders);
		}

		public async Task<IEnumerable<SalesOrderResponseDto>> GetAllSalesOrdersByProcessFlagAsync(int processFlag)
		{
			_logger.LogDebug("Getting all sales orders with process flag: {ProcessFlag}", processFlag);
			var salesOrders = await _context.SalesOrders
				.Include(so => so.Items)
				.Where(so => so.ProcessFlag == processFlag)
				.ToListAsync();
			_logger.LogInformation("Retrieved {SalesOrderCount} sales orders with process flag: {ProcessFlag}", salesOrders.Count(), processFlag);
			return _mapper.Map<IEnumerable<SalesOrderResponseDto>>(salesOrders);
		}

		public async Task<SalesOrderResponseDto?> GetSalesOrderByIdAsync(int id)
		{
			_logger.LogDebug("Getting sales order by ID: {SalesOrderId}", id);
			var salesOrder = await _context.SalesOrders
				.Include(so => so.Items)
				.FirstOrDefaultAsync(so => so.Id == id);
			if (salesOrder == null)
			{
				_logger.LogWarning("Sales order {SalesOrderId} not found", id);
				return null;
			}
			return _mapper.Map<SalesOrderResponseDto>(salesOrder);
		}

		public async Task<SalesOrderResponseDto> CreateSalesOrderAsync(CreateSalesOrderRequestDto createSalesOrderDto)
		{
			_logger.LogDebug("Creating new sales order: {VoucherNumber}", createSalesOrderDto.VoucherNumber);

			// Check if voucher number is unique
			var existingSalesOrder = await _salesOrderRepository.FirstOrDefaultAsync(so => so.VoucherNumber == createSalesOrderDto.VoucherNumber);
			if (existingSalesOrder != null)
			{
				throw new InvalidOperationException($"Sales order with voucher number '{createSalesOrderDto.VoucherNumber}' already exists");
			}

			var salesOrder = _mapper.Map<SalesOrder>(createSalesOrderDto);

			// Map items
			salesOrder.Items = _mapper.Map<ICollection<SalesOrderItem>>(createSalesOrderDto.Items);

			await _salesOrderRepository.AddAsync(salesOrder);
			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("Created sales order {SalesOrderId} with voucher number: {VoucherNumber}", salesOrder.Id, salesOrder.VoucherNumber);

			// Load the complete entity with items for mapping
			var completeSalesOrder = await _context.SalesOrders
				.Include(so => so.Items)
				.FirstAsync(so => so.Id == salesOrder.Id);
			return _mapper.Map<SalesOrderResponseDto>(completeSalesOrder);
		}

		public async Task<SalesOrderResponseDto?> UpdateSalesOrderAsync(int id, UpdateSalesOrderRequestDto updateSalesOrderDto)
		{
			_logger.LogDebug("Updating sales order {SalesOrderId}", id);

			var salesOrder = await _context.SalesOrders
				.Include(so => so.Items)
				.FirstOrDefaultAsync(so => so.Id == id);
			if (salesOrder == null)
			{
				_logger.LogWarning("Sales order {SalesOrderId} not found for update", id);
				return null;
			}

			// Check if voucher number is unique (excluding current sales order)
			var existingSalesOrder = await _salesOrderRepository.FirstOrDefaultAsync(so =>
				so.VoucherNumber == updateSalesOrderDto.VoucherNumber && so.Id != id);
			if (existingSalesOrder != null)
			{
				throw new InvalidOperationException($"Sales order with voucher number '{updateSalesOrderDto.VoucherNumber}' already exists");
			}

			// Update main sales order properties
			_mapper.Map(updateSalesOrderDto, salesOrder);
			salesOrder.UpdatedAt = DateTime.Now;

			// Update items
			UpdateSalesOrderItems(salesOrder, updateSalesOrderDto.Items);

			_salesOrderRepository.Update(salesOrder);
			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("Updated sales order {SalesOrderId}", id);

			// Load the complete entity with items for mapping
			var completeSalesOrder = await _context.SalesOrders
				.Include(so => so.Items)
				.FirstAsync(so => so.Id == salesOrder.Id);
			return _mapper.Map<SalesOrderResponseDto>(completeSalesOrder);
		}

		public async Task<bool> DeleteSalesOrderAsync(int id)
		{
			_logger.LogDebug("Deleting sales order {SalesOrderId}", id);

			var salesOrder = await _salesOrderRepository.GetByIdAsync(id);
			if (salesOrder == null)
			{
				_logger.LogWarning("Sales order {SalesOrderId} not found for deletion", id);
				return false;
			}
			_salesOrderRepository.Remove(salesOrder);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<SalesOrderResponseDto>> SearchSalesOrdersAsync(SalesOrderSearchRequestDto searchDto)
		{
			_logger.LogDebug("Searching sales orders with voucher number: {VoucherNumber}", searchDto.VoucherNumber);

			var query = _context.SalesOrders.AsQueryable();

			if (!string.IsNullOrEmpty(searchDto.VoucherNumber))
				query = query.Where(so => so.VoucherNumber.Contains(searchDto.VoucherNumber));

			if (!string.IsNullOrEmpty(searchDto.PartyName))
				query = query.Where(so => so.PartyName.Contains(searchDto.PartyName));

			if (searchDto.FromDate.HasValue)
				query = query.Where(so => so.SalesDate >= searchDto.FromDate.Value);

			if (searchDto.ToDate.HasValue)
				query = query.Where(so => so.SalesDate <= searchDto.ToDate.Value);

			if (searchDto.ProcessFlag.HasValue)
				query = query.Where(so => so.ProcessFlag == searchDto.ProcessFlag.Value);

			var salesOrders = await query
				.Include(so => so.Items)
				.ToListAsync();

			return _mapper.Map<IEnumerable<SalesOrderResponseDto>>(salesOrders);
		}

		public async Task<bool> IsVoucherNumberUniqueAsync(string voucherNumber, int? excludeId = null)
		{
			var query = await _salesOrderRepository.FindAsync(so => so.VoucherNumber == voucherNumber);
			if (excludeId.HasValue)
			{
				query = query.Where(so => so.Id != excludeId.Value);
			}
			return !query.Any();
		}

		public async Task<IEnumerable<SalesOrderResponseDto>> GetUnprocessedSalesOrdersAsync()
		{
			_logger.LogDebug("Getting unprocessed sales orders");

			var salesOrders = await _context.SalesOrders
				.Include(so => so.Items)
				.Where(so => so.ProcessFlag == 0)
				.ToListAsync();
			return _mapper.Map<IEnumerable<SalesOrderResponseDto>>(salesOrders);
		}

		public async Task<bool> MarkAsProcessedAsync(int id)
		{
			_logger.LogDebug("Marking sales order {SalesOrderId} as processed", id);

			var salesOrder = await _salesOrderRepository.GetByIdAsync(id);
			if (salesOrder == null)
			{
				_logger.LogWarning("Sales order {SalesOrderId} not found for processing", id);
				return false;
			}

			salesOrder.ProcessFlag = 1;
			salesOrder.ProcessDate = DateTime.Now;
			salesOrder.UpdatedAt = DateTime.Now;

			_salesOrderRepository.Update(salesOrder);
			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("Marked sales order {SalesOrderId} as processed", id);
			return true;
		}

		public async Task<bool> MarkSalesOrderItemAsProcessedAsync(int salesOrderId, int salesOrderItemId)
		{
			_logger.LogDebug("Marking sales order item {SalesOrderItemId} as processed for sales order {SalesOrderId}", salesOrderItemId, salesOrderId);

			var salesOrder = await _context.SalesOrders
				.Include(so => so.Items)
				.FirstOrDefaultAsync(so => so.Id == salesOrderId);

			if (salesOrder == null)
			{
				_logger.LogWarning("Sales order {SalesOrderId} not found for processing item {SalesOrderItemId}", salesOrderId, salesOrderItemId);
				return false;
			}

			var salesOrderItem = salesOrder.Items.FirstOrDefault(item => item.Id == salesOrderItemId);
			if (salesOrderItem == null)
			{
				_logger.LogWarning("Sales order item {SalesOrderItemId} not found in sales order {SalesOrderId}", salesOrderItemId, salesOrderId);
				return false;
			}

			// Update the item's process flag
			salesOrderItem.ProcessFlag = 1;
			salesOrderItem.ProcessDate = DateTime.Now;

			// Check if all items are processed, then mark the entire order as processed
			if (salesOrder.Items.All(item => item.ProcessFlag == 1))
			{
				salesOrder.ProcessFlag = 1;
				salesOrder.ProcessDate = DateTime.Now;
			}

			salesOrder.UpdatedAt = DateTime.Now;

			_salesOrderRepository.Update(salesOrder);
			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("Marked sales order item {SalesOrderItemId} as processed for sales order {SalesOrderId}", salesOrderItemId, salesOrderId);
			return true;
		}

		private void UpdateSalesOrderItems(SalesOrder salesOrder, ICollection<UpdateSalesOrderItemRequestDto> itemDtos)
		{
			// Remove items that are not in the update DTOs (if they have IDs)
			var itemsToRemove = salesOrder.Items
				.Where(item => itemDtos.All(dto => !dto.Id.HasValue || dto.Id.Value != item.Id))
				.ToList();

			foreach (var item in itemsToRemove)
			{
				salesOrder.Items.Remove(item);
			}

			// Update or add items
			foreach (var itemDto in itemDtos)
			{
				if (itemDto.Id.HasValue && itemDto.Id.Value > 0)
				{
					// Update existing item
					var existingItem = salesOrder.Items.FirstOrDefault(i => i.Id == itemDto.Id.Value);
					if (existingItem != null)
					{
						_mapper.Map(itemDto, existingItem);
					}
				}
				else
				{
					// Add new item
					var newItem = _mapper.Map<SalesOrderItem>(itemDto);
					salesOrder.Items.Add(newItem);
				}
			}
		}
	}
}