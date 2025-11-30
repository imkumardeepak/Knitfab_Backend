using AvyyanBackend.Data;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Repositories
{
    public class SalesOrderWebRepository : ISalesOrderWebRepository
    {
        private readonly ApplicationDbContext _context;

        public SalesOrderWebRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesOrderWeb>> GetAllAsync()
        {
            return await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .ToListAsync();
        }

        public async Task<SalesOrderWeb?> GetByIdAsync(int id)
        {
            return await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .FirstOrDefaultAsync(sow => sow.Id == id);
        }

        public async Task<SalesOrderWeb> CreateAsync(SalesOrderWeb salesOrderWeb)
        {
            _context.SalesOrdersWeb.Add(salesOrderWeb);
            await _context.SaveChangesAsync();
            return salesOrderWeb;
        }

        public async Task<SalesOrderWeb?> UpdateAsync(int id, SalesOrderWeb salesOrderWeb)
        {
            var existingSalesOrderWeb = await _context.SalesOrdersWeb
                .Include(sow => sow.Items)
                .FirstOrDefaultAsync(sow => sow.Id == id);

            if (existingSalesOrderWeb == null)
                return null;

            // Update main properties
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
            existingSalesOrderWeb.VoucherNumber = salesOrderWeb.VoucherNumber;
            existingSalesOrderWeb.OrderDate = salesOrderWeb.OrderDate;
            existingSalesOrderWeb.TermsOfPayment = salesOrderWeb.TermsOfPayment;
            existingSalesOrderWeb.IsJobWork = salesOrderWeb.IsJobWork;
            existingSalesOrderWeb.Remarks = salesOrderWeb.Remarks;
            existingSalesOrderWeb.UpdatedAt = DateTime.Now;

            // Remove items that are not in the updated list
            var itemsToRemove = existingSalesOrderWeb.Items
                .Where(item => salesOrderWeb.Items.All(newItem => newItem.Id != item.Id))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                _context.SalesOrderItemsWeb.Remove(item);
            }

            // Update or add items
            foreach (var item in salesOrderWeb.Items)
            {
                if (item.Id > 0)
                {
                    // Update existing item
                    var existingItem = existingSalesOrderWeb.Items.FirstOrDefault(i => i.Id == item.Id);
                    if (existingItem != null)
                    {
                        existingItem.ItemName = item.ItemName;
                        existingItem.ItemDescription = item.ItemDescription;
                        existingItem.YarnCount = item.YarnCount;
                        existingItem.Dia = item.Dia;
                        existingItem.GG = item.GG;
                        existingItem.FabricType = item.FabricType;
                        existingItem.Composition = item.Composition;
                        existingItem.WtPerRoll = item.WtPerRoll;
                        existingItem.NoOfRolls = item.NoOfRolls;
                        existingItem.Rate = item.Rate;
                        existingItem.Qty = item.Qty;
                        existingItem.Amount = item.Amount;
                        existingItem.IGST = item.IGST;
                        existingItem.SGST = item.SGST;
                        existingItem.Remarks = item.Remarks;
                    }
                }
                else
                {
                    // Add new item
                    existingSalesOrderWeb.Items.Add(item);
                }
            }

            await _context.SaveChangesAsync();
            return existingSalesOrderWeb;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var salesOrderWeb = await _context.SalesOrdersWeb.FindAsync(id);
            if (salesOrderWeb == null)
                return false;

            _context.SalesOrdersWeb.Remove(salesOrderWeb);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.SalesOrdersWeb.AnyAsync(sow => sow.Id == id);
        }
    }
}