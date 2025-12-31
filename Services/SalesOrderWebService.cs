using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.SalesOrder;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
    public class SalesOrderWebService : ISalesOrderWebService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SalesOrderWebService> _logger;

        public SalesOrderWebService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<SalesOrderWebService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SalesOrderWebResponseDto>> GetAllAsync()
        {
            _logger.LogDebug("Getting all sales orders web");
            var salesOrdersWeb = await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .ToListAsync();
            _logger.LogInformation("Retrieved {SalesOrderWebCount} sales orders web", salesOrdersWeb.Count());
            return _mapper.Map<IEnumerable<SalesOrderWebResponseDto>>(salesOrdersWeb);
        }

        public async Task<SalesOrderWebResponseDto?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Getting sales order web by ID: {SalesOrderWebId}", id);
            var salesOrderWeb = await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .FirstOrDefaultAsync(sow => sow.Id == id);
            if (salesOrderWeb == null)
            {
                _logger.LogWarning("Sales order web {SalesOrderWebId} not found", id);
                return null;
            }
            return _mapper.Map<SalesOrderWebResponseDto>(salesOrderWeb);
        }

        public async Task<SalesOrderWebResponseDto> CreateAsync(CreateSalesOrderWebRequestDto createSalesOrderWebDto)
        {
            _logger.LogDebug("Creating new sales order web: {VoucherNumber}", createSalesOrderWebDto.VoucherNumber);

            // Validate voucher number format
            if (!IsValidVoucherNumber(createSalesOrderWebDto.VoucherNumber))
            {
                throw new InvalidOperationException("Invalid voucher number format. Voucher number must contain either \"/J\" or \"/A\".");
            }

            // Check if voucher number is unique
            var existingSalesOrderWeb = await _context.SalesOrdersWeb
                .FirstOrDefaultAsync(sow => sow.VoucherNumber == createSalesOrderWebDto.VoucherNumber);
            if (existingSalesOrderWeb != null)
            {
                //return StatusCode(StatusCodes.Status500InternalServerError,
                    //new { message = $"Sales order web with voucher number '{createSalesOrderWebDto.VoucherNumber}' already exists" });
                throw new InvalidOperationException($"Sales order web with voucher number '{createSalesOrderWebDto.VoucherNumber}' already exists");
            }

            // Generate serial number if not provided
            if (string.IsNullOrEmpty(createSalesOrderWebDto.SerialNo))
            {
                createSalesOrderWebDto.SerialNo = await GetNextSerialNumberAsync();
            }

            // Calculate totals if not provided
            if (createSalesOrderWebDto.TotalQuantity == 0 && createSalesOrderWebDto.TotalAmount == 0)
            {
                createSalesOrderWebDto.TotalQuantity = createSalesOrderWebDto.Items.Sum(item => item.Qty);
                createSalesOrderWebDto.TotalAmount = createSalesOrderWebDto.Items.Sum(item => item.Amount);
            }

            var salesOrderWeb = _mapper.Map<SalesOrderWeb>(createSalesOrderWebDto);

            // Map items
            salesOrderWeb.Items = _mapper.Map<ICollection<SalesOrderItemWeb>>(createSalesOrderWebDto.Items);

            // Set audit fields
            salesOrderWeb.CreatedAt = DateTime.UtcNow;
            salesOrderWeb.UpdatedAt = DateTime.UtcNow;
            // CreatedBy should already be mapped from the DTO, but ensure it's set if needed
            if (string.IsNullOrEmpty(salesOrderWeb.CreatedBy))
            {
                salesOrderWeb.CreatedBy = "System"; // Default value if not provided
            }
            salesOrderWeb.UpdatedBy = salesOrderWeb.CreatedBy; // Initially the same as created by

            _context.SalesOrdersWeb.Add(salesOrderWeb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created sales order web {SalesOrderWebId} with voucher number: {VoucherNumber}", salesOrderWeb.Id, salesOrderWeb.VoucherNumber);

            // Load the complete entity with items for mapping
            var completeSalesOrderWeb = await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .FirstAsync(sow => sow.Id == salesOrderWeb.Id);
            return _mapper.Map<SalesOrderWebResponseDto>(completeSalesOrderWeb);
        }

        public async Task<SalesOrderWebResponseDto?> UpdateAsync(int id, UpdateSalesOrderWebRequestDto updateSalesOrderWebDto)
        {
            _logger.LogDebug("Updating sales order web {SalesOrderWebId}", id);

            // Validate voucher number format
            if (!IsValidVoucherNumber(updateSalesOrderWebDto.VoucherNumber))
            {
                throw new InvalidOperationException("Invalid voucher number format. Voucher number must contain either \"/J\" or \"/A\".");
            }

            var salesOrderWeb = await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .FirstOrDefaultAsync(sow => sow.Id == id);
            if (salesOrderWeb == null)
            {
                _logger.LogWarning("Sales order web {SalesOrderWebId} not found for update", id);
                return null;
            }

            // Check if voucher number is unique (excluding current sales order)
            var existingSalesOrderWeb = await _context.SalesOrdersWeb
                .FirstOrDefaultAsync(sow => sow.VoucherNumber == updateSalesOrderWebDto.VoucherNumber && sow.Id != id);
            if (existingSalesOrderWeb != null)
            {
                throw new InvalidOperationException($"Sales order web with voucher number '{updateSalesOrderWebDto.VoucherNumber}' already exists");
            }

            // Generate serial number if not provided
            if (string.IsNullOrEmpty(updateSalesOrderWebDto.SerialNo))
            {
                updateSalesOrderWebDto.SerialNo = await GetNextSerialNumberAsync();
            }

            // Calculate totals if not provided
            if (updateSalesOrderWebDto.TotalQuantity == 0 && updateSalesOrderWebDto.TotalAmount == 0)
            {
                updateSalesOrderWebDto.TotalQuantity = updateSalesOrderWebDto.Items.Sum(item => item.Qty);
                updateSalesOrderWebDto.TotalAmount = updateSalesOrderWebDto.Items.Sum(item => item.Amount);
            }

			// Update items
			UpdateSalesOrderWebItems(salesOrderWeb, updateSalesOrderWebDto.Items);

			// Update main sales order properties
			_mapper.Map(updateSalesOrderWebDto, salesOrderWeb);
            salesOrderWeb.UpdatedAt = DateTime.UtcNow;
            // UpdatedBy should already be mapped from the DTO, but ensure it's set
            if (string.IsNullOrEmpty(salesOrderWeb.UpdatedBy))
            {
                salesOrderWeb.UpdatedBy = "System"; // Default value if not provided
            }

            _context.SalesOrdersWeb.Update(salesOrderWeb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated sales order web {SalesOrderWebId}", id);

            // Load the complete entity with items for mapping
            var completeSalesOrderWeb = await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .FirstAsync(sow => sow.Id == salesOrderWeb.Id);
            return _mapper.Map<SalesOrderWebResponseDto>(completeSalesOrderWeb);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogDebug("Deleting sales order web {SalesOrderWebId}", id);

            var salesOrderWeb = await _context.SalesOrdersWeb.FindAsync(id);
            if (salesOrderWeb == null)
            {
                _logger.LogWarning("Sales order web {SalesOrderWebId} not found for deletion", id);
                return false;
            }

            _context.SalesOrdersWeb.Remove(salesOrderWeb);
            await _context.SaveChangesAsync();
            return true;
        }

        // Method to mark a sales order web item as processed
        public async Task<SalesOrderWebResponseDto?> MarkSalesOrderItemWebAsProcessedAsync(int salesOrderWebId, int salesOrderItemWebId)
        {
            _logger.LogDebug("Marking sales order web item {SalesOrderItemWebId} as processed for sales order web {SalesOrderWebId}", salesOrderItemWebId, salesOrderWebId);

            var salesOrderWeb = await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .FirstOrDefaultAsync(sow => sow.Id == salesOrderWebId);



            if (salesOrderWeb == null)
            {
                _logger.LogWarning("Sales order web {SalesOrderWebId} not found for processing item {SalesOrderItemWebId}", salesOrderWebId, salesOrderItemWebId);
                return null;
            }

            var salesOrderItemWeb = salesOrderWeb.Items.FirstOrDefault(item => item.Id == salesOrderItemWebId);
            if (salesOrderItemWeb == null)
            {
                _logger.LogWarning("Sales order web item {SalesOrderItemWebId} not found in sales order web {SalesOrderWebId}", salesOrderItemWebId, salesOrderWebId);
                return null;
            }

            // Update the item's process flag
            salesOrderItemWeb.IsProcess = true;
            salesOrderItemWeb.ProcessDate = DateTime.UtcNow;

            // Check if all items are processed, then mark the entire order as processed
            if (salesOrderWeb.Items.All(item => item.IsProcess == true))
            {
                salesOrderWeb.IsProcess = true;
                salesOrderWeb.ProcessDate = DateTime.UtcNow;
            }

            salesOrderWeb.UpdatedAt = DateTime.UtcNow;

            _context.SalesOrdersWeb.Update(salesOrderWeb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes while marking sales order web item {SalesOrderItemWebId} as processed", salesOrderItemWebId);
                throw; // Re-throw to let the controller handle it
            }
            

            _logger.LogInformation("Marked sales order web item {SalesOrderItemWebId} as processed for sales order web {SalesOrderWebId}", salesOrderItemWebId, salesOrderWebId);
            
            // Return the updated order
            return _mapper.Map<SalesOrderWebResponseDto>(salesOrderWeb);
        }

        private void UpdateSalesOrderWebItems(SalesOrderWeb salesOrderWeb, ICollection<UpdateSalesOrderItemWebRequestDto> itemDtos)
        {
            // Remove items that are not in the update DTOs (if they have IDs)
            var itemsToRemove = salesOrderWeb.Items
                .Where(item => itemDtos.All(dto => !dto.Id.HasValue || dto.Id.Value != item.Id))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                salesOrderWeb.Items.Remove(item);
            }

            // Update or add items
            foreach (var itemDto in itemDtos)
            {
                if (itemDto.Id.HasValue && itemDto.Id.Value > 0)
                {
                    // Update existing item
                    var existingItem = salesOrderWeb.Items.FirstOrDefault(i => i.Id == itemDto.Id.Value);
                    if (existingItem != null)
                    {
                        _mapper.Map(itemDto, existingItem);
                    }
                }
                else
                {
                    // Add new item
                    var newItem = _mapper.Map<SalesOrderItemWeb>(itemDto);
                    salesOrderWeb.Items.Add(newItem);
                }
            }
        }

        // Method to generate the next serial number for sales orders
        public async Task<string> GetNextSerialNumberAsync()
        {
            try
            {
                // Get the maximum serial number from existing sales orders and add 1
                var maxSerialNumber = 0;
                var existingOrders = await _context.SalesOrdersWeb
                    .Select(order => order.SerialNo)
                    .ToListAsync();

                if (existingOrders.Any())
                {
                    foreach (var serialNo in existingOrders)
                    {
                        // Parse the serial number (format: "0001", "0002", etc.)
                        if (int.TryParse(serialNo, out int serial))
                        {
                            if (serial > maxSerialNumber)
                                maxSerialNumber = serial;
                        }
                    }
                }

                var nextNumber = maxSerialNumber + 1;

                // Format as 4-digit zero-padded string
                var serialNumber = nextNumber.ToString("D4");

                return serialNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating next serial number");
                throw new InvalidOperationException("Error generating next serial number", ex);
            }
        }

        // Method to generate voucher number based on job work flag and financial year
        public string GenerateVoucherNumber(bool isJobWork, string buyerName)
        {
            // Get current financial year (e.g., 24-25)
            var currentYear = DateTime.UtcNow.Year;
            var currentMonth = DateTime.UtcNow.Month;

            // Financial year logic: April to March
            int financialYearStart, financialYearEnd;
            if (currentMonth >= 4) // April or later
            {
                financialYearStart = currentYear % 100; // Last two digits of current year
                financialYearEnd = (currentYear + 1) % 100; // Last two digits of next year
            }
            else // January to March
            {
                financialYearStart = (currentYear - 1) % 100; // Last two digits of previous year
                financialYearEnd = currentYear % 100; // Last two digits of current year
            }

            var financialYear = $"{financialYearStart:D2}-{financialYearEnd:D2}"; // Format as 24-25

            // Determine series: A for customer work, J for job work
            var series = isJobWork ? "J" : "A";

            // Get the next serial number for this combination
            var nextSerial = GetNextSerialNumber(isJobWork, buyerName);

            // Format the voucher number: AKF/24-25/A0001
            return $"AKF/{financialYear}/{series}{nextSerial:D4}";
        }

        // Method to get the next serial number
        private int GetNextSerialNumber(bool isJobWork, string buyerName)
        {
            // Get the last voucher number for the same series (A or J)
            var series = isJobWork ? "J" : "A";

            // Find the highest voucher number with the same series
            // We'll fetch all matching records and process them in memory
            var matchingOrders = _context.SalesOrdersWeb
                .Where(sow => sow.VoucherNumber.Contains($"/{series}"))
                .AsEnumerable() // Switch to client evaluation for complex string operations
                .Where(sow => sow.VoucherNumber.Split('/').Last().StartsWith(series))
                .OrderByDescending(sow => sow.CreatedAt)
                .ToList();

            var lastOrder = matchingOrders.FirstOrDefault();

            if (lastOrder != null)
            {
                // Extract the serial number from the last voucher number and increment
                var lastVoucherNumber = lastOrder.VoucherNumber;
                var parts = lastVoucherNumber.Split('/');
                if (parts.Length == 3)
                {
                    var serialPart = parts[2]; // Get the last part (e.g., "A0001")
                    if (serialPart.Length > 1 && char.ToUpper(serialPart[0]) == series[0])
                    {
                        var numericPart = serialPart.Substring(1); // Remove the series letter (e.g., "0001")
                        if (int.TryParse(numericPart, out int lastSerial))
                        {
                            return lastSerial + 1;
                        }
                    }
                }
            }

            // If no previous order found, start with 1
            return 1;
        }

        // Helper method to extract the last part of a voucher number
        private string GetLastPart(string voucherNumber)
        {
            var parts = voucherNumber.Split('/');
            return parts.Length > 0 ? parts[parts.Length - 1] : string.Empty;
        }

        // Helper method to validate voucher number format
        private bool IsValidVoucherNumber(string voucherNumber)
        {
            return voucherNumber.Contains("/J") || voucherNumber.Contains("/A");
        }
    }
}