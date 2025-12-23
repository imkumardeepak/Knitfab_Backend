using AvyyanBackend.Data;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Repositories
{
    /// <summary>
    /// Repository for SalesOrderWeb data access operations
    /// </summary>
    public class SalesOrderWebRepository : ISalesOrderWebRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SalesOrderWebRepository> _logger;

        public SalesOrderWebRepository(
            ApplicationDbContext context,
            ILogger<SalesOrderWebRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all sales orders web with items
        /// </summary>
        public async Task<IEnumerable<SalesOrderWeb>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Fetching all sales orders web from database");
                
                return await _context.SalesOrdersWeb
                    .Include(sow => sow.Items)
                    .OrderByDescending(sow => sow.CreatedAt)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all sales orders web");
                throw;
            }
        }

        /// <summary>
        /// Get sales order web by ID with items
        /// </summary>
        public async Task<SalesOrderWeb?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching sales order web {SalesOrderWebId} from database", id);
                
                return await _context.SalesOrdersWeb
                    .Include(sow => sow.Items)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sow => sow.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sales order web {SalesOrderWebId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new sales order web
        /// </summary>
        public async Task<SalesOrderWeb> CreateAsync(SalesOrderWeb salesOrderWeb)
        {
            try
            {
                if (salesOrderWeb == null)
                {
                    throw new ArgumentNullException(nameof(salesOrderWeb));
                }

                _logger.LogDebug("Creating new sales order web with voucher number: {VoucherNumber}", 
                    salesOrderWeb.VoucherNumber);

                // Set timestamps
                salesOrderWeb.CreatedAt = DateTime.UtcNow;
                salesOrderWeb.UpdatedAt = DateTime.UtcNow;

                _context.SalesOrdersWeb.Add(salesOrderWeb);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created sales order web {SalesOrderWebId}", 
                    salesOrderWeb.Id);

                return salesOrderWeb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sales order web");
                throw;
            }
        }

        /// <summary>
        /// Update an existing sales order web
        /// </summary>
        public async Task<SalesOrderWeb?> UpdateAsync(int id, SalesOrderWeb salesOrderWeb)
        {
            try
            {
                if (salesOrderWeb == null)
                {
                    throw new ArgumentNullException(nameof(salesOrderWeb));
                }

                _logger.LogDebug("Updating sales order web {SalesOrderWebId}", id);

                var existingSalesOrderWeb = await _context.SalesOrdersWeb
                    .Include(sow => sow.Items)
                    .FirstOrDefaultAsync(sow => sow.Id == id);

                if (existingSalesOrderWeb == null)
                {
                    _logger.LogWarning("Sales order web {SalesOrderWebId} not found for update", id);
                    return null;
                }

                // Update all main properties
                existingSalesOrderWeb.VoucherType = salesOrderWeb.VoucherType;
                existingSalesOrderWeb.VoucherNumber = salesOrderWeb.VoucherNumber;
                existingSalesOrderWeb.OrderDate = salesOrderWeb.OrderDate;
                existingSalesOrderWeb.TermsOfPayment = salesOrderWeb.TermsOfPayment;
                existingSalesOrderWeb.IsJobWork = salesOrderWeb.IsJobWork;
                existingSalesOrderWeb.SerialNo = salesOrderWeb.SerialNo;
                existingSalesOrderWeb.IsProcess = salesOrderWeb.IsProcess;
                existingSalesOrderWeb.OrderNo = salesOrderWeb.OrderNo;
                existingSalesOrderWeb.TermsOfDelivery = salesOrderWeb.TermsOfDelivery;
                existingSalesOrderWeb.DispatchThrough = salesOrderWeb.DispatchThrough;
                existingSalesOrderWeb.CompanyName = salesOrderWeb.CompanyName;
                existingSalesOrderWeb.CompanyGSTIN = salesOrderWeb.CompanyGSTIN;
                existingSalesOrderWeb.CompanyState = salesOrderWeb.CompanyState;
                existingSalesOrderWeb.BuyerName = salesOrderWeb.BuyerName;
                existingSalesOrderWeb.BuyerGSTIN = salesOrderWeb.BuyerGSTIN;
                existingSalesOrderWeb.BuyerState = salesOrderWeb.BuyerState;
                existingSalesOrderWeb.BuyerPhone = salesOrderWeb.BuyerPhone;
                existingSalesOrderWeb.BuyerContactPerson = salesOrderWeb.BuyerContactPerson;
                existingSalesOrderWeb.BuyerAddress = salesOrderWeb.BuyerAddress;
                existingSalesOrderWeb.ConsigneeName = salesOrderWeb.ConsigneeName;
                existingSalesOrderWeb.ConsigneeGSTIN = salesOrderWeb.ConsigneeGSTIN;
                existingSalesOrderWeb.ConsigneeState = salesOrderWeb.ConsigneeState;
                existingSalesOrderWeb.ConsigneePhone = salesOrderWeb.ConsigneePhone;
                existingSalesOrderWeb.ConsigneeContactPerson = salesOrderWeb.ConsigneeContactPerson;
                existingSalesOrderWeb.ConsigneeAddress = salesOrderWeb.ConsigneeAddress;
                existingSalesOrderWeb.Remarks = salesOrderWeb.Remarks;
                existingSalesOrderWeb.OtherReference = salesOrderWeb.OtherReference;
                existingSalesOrderWeb.TotalQuantity = salesOrderWeb.TotalQuantity;
                existingSalesOrderWeb.TotalAmount = salesOrderWeb.TotalAmount;
                existingSalesOrderWeb.ProcessDate = salesOrderWeb.ProcessDate;
                existingSalesOrderWeb.UpdatedAt = DateTime.UtcNow;
                existingSalesOrderWeb.UpdatedBy = salesOrderWeb.UpdatedBy;

                // Update items collection
                UpdateItems(existingSalesOrderWeb, salesOrderWeb.Items);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated sales order web {SalesOrderWebId}", id);

                return existingSalesOrderWeb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sales order web {SalesOrderWebId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a sales order web
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogDebug("Deleting sales order web {SalesOrderWebId}", id);

                var salesOrderWeb = await _context.SalesOrdersWeb
                    .Include(sow => sow.Items)
                    .FirstOrDefaultAsync(sow => sow.Id == id);

                if (salesOrderWeb == null)
                {
                    _logger.LogWarning("Sales order web {SalesOrderWebId} not found for deletion", id);
                    return false;
                }

                _context.SalesOrdersWeb.Remove(salesOrderWeb);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted sales order web {SalesOrderWebId}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sales order web {SalesOrderWebId}", id);
                throw;
            }
        }

        /// <summary>
        /// Check if a sales order web exists
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.SalesOrdersWeb.AnyAsync(sow => sow.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of sales order web {SalesOrderWebId}", id);
                throw;
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Update items collection - handles add, update, and delete
        /// </summary>
        private void UpdateItems(SalesOrderWeb existingSalesOrderWeb, ICollection<SalesOrderItemWeb> newItems)
        {
            // Remove items that are not in the updated list
            var itemsToRemove = existingSalesOrderWeb.Items
                .Where(item => newItems.All(newItem => newItem.Id != item.Id))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                _logger.LogDebug("Removing sales order item {ItemId}", item.Id);
                _context.SalesOrderItemsWeb.Remove(item);
            }

            // Update or add items
            foreach (var item in newItems)
            {
                if (item.Id > 0)
                {
                    // Update existing item
                    var existingItem = existingSalesOrderWeb.Items.FirstOrDefault(i => i.Id == item.Id);
                    if (existingItem != null)
                    {
                        _logger.LogDebug("Updating sales order item {ItemId}", item.Id);
                        UpdateItemProperties(existingItem, item);
                    }
                }
                else
                {
                    // Add new item
                    _logger.LogDebug("Adding new sales order item");
                    item.SalesOrderWebId = existingSalesOrderWeb.Id;
                    existingSalesOrderWeb.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Update all properties of an existing item
        /// </summary>
        private void UpdateItemProperties(SalesOrderItemWeb existingItem, SalesOrderItemWeb newItem)
        {
            existingItem.ItemName = newItem.ItemName;
            existingItem.HSNCode = newItem.HSNCode;
            existingItem.YarnCount = newItem.YarnCount;
            existingItem.Dia = newItem.Dia;
            existingItem.GG = newItem.GG;
            existingItem.FabricType = newItem.FabricType;
            existingItem.Composition = newItem.Composition;
            existingItem.WtPerRoll = newItem.WtPerRoll;
            existingItem.NoOfRolls = newItem.NoOfRolls;
            existingItem.Rate = newItem.Rate;
            existingItem.Qty = newItem.Qty;
            existingItem.Amount = newItem.Amount;
            existingItem.IGST = newItem.IGST;
            existingItem.SGST = newItem.SGST;
            existingItem.CGST = newItem.CGST;
            existingItem.Remarks = newItem.Remarks;
            existingItem.Unit = newItem.Unit;
            existingItem.SlitLine = newItem.SlitLine;
            existingItem.StitchLength = newItem.StitchLength;
            existingItem.DueDate = newItem.DueDate;
            existingItem.IsProcess = newItem.IsProcess;
            existingItem.ProcessDate = newItem.ProcessDate;
        }

        #endregion
    }
}