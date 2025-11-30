using AvyyanBackend.Data;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Repositories
{
    public class DispatchPlanningRepository : IDispatchPlanningRepository
    {
        private readonly ApplicationDbContext _context;

        public DispatchPlanningRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DispatchPlanning> CreateAsync(DispatchPlanning dispatchPlanning)
        {
            _context.DispatchPlannings.Add(dispatchPlanning);
            await _context.SaveChangesAsync();
            return dispatchPlanning;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dispatchPlanning = await _context.DispatchPlannings.FindAsync(id);
            if (dispatchPlanning == null)
                return false;

            _context.DispatchPlannings.Remove(dispatchPlanning);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DispatchPlanning>> GetAllAsync()
        {
            return await _context.DispatchPlannings
                .Include(dp => dp.Transport)
                .Include(dp => dp.Courier)
                .ToListAsync();
        }

        public async Task<DispatchPlanning?> GetByIdAsync(int id)
        {
            return await _context.DispatchPlannings
                .Include(dp => dp.Transport)
                .Include(dp => dp.Courier)
                .FirstOrDefaultAsync(dp => dp.Id == id);
        }

        public async Task<DispatchPlanning?> GetByLotNoAsync(string lotNo)
        {
            return await _context.DispatchPlannings
                .Include(dp => dp.Transport)
                .Include(dp => dp.Courier)
                .FirstOrDefaultAsync(dp => dp.LotNo == lotNo);
        }

        public async Task<DispatchPlanning> UpdateAsync(int id, DispatchPlanning dispatchPlanning)
        {
            var existing = await _context.DispatchPlannings.FindAsync(id);
            if (existing == null)
                throw new Exception("DispatchPlanning not found");

            // Update properties
            existing.LotNo = dispatchPlanning.LotNo;
            existing.SalesOrderId = dispatchPlanning.SalesOrderId;
            existing.SalesOrderItemId = dispatchPlanning.SalesOrderItemId;
            existing.CustomerName = dispatchPlanning.CustomerName;
            existing.Tape = dispatchPlanning.Tape;
            existing.TotalRequiredRolls = dispatchPlanning.TotalRequiredRolls;
            existing.TotalReadyRolls = dispatchPlanning.TotalReadyRolls;
            existing.TotalDispatchedRolls = dispatchPlanning.TotalDispatchedRolls;
            existing.IsFullyDispatched = dispatchPlanning.IsFullyDispatched;
            existing.DispatchStartDate = dispatchPlanning.DispatchStartDate;
            existing.DispatchEndDate = dispatchPlanning.DispatchEndDate;
            existing.VehicleNo = dispatchPlanning.VehicleNo;
            existing.DriverName = dispatchPlanning.DriverName;
            existing.License = dispatchPlanning.License;
            existing.MobileNumber = dispatchPlanning.MobileNumber;
            existing.Remarks = dispatchPlanning.Remarks;
            existing.LoadingNo = dispatchPlanning.LoadingNo;
            existing.DispatchOrderId = dispatchPlanning.DispatchOrderId;
            
            // Transport/Courier fields
            existing.IsTransport = dispatchPlanning.IsTransport;
            existing.IsCourier = dispatchPlanning.IsCourier;
            existing.TransportId = dispatchPlanning.TransportId;
            existing.CourierId = dispatchPlanning.CourierId;
            // Manual transport details (new fields)
            existing.TransportName = dispatchPlanning.TransportName;
            existing.ContactPerson = dispatchPlanning.ContactPerson;
            existing.Phone = dispatchPlanning.Phone;
            existing.MaximumCapacityKgs = dispatchPlanning.MaximumCapacityKgs;
            // Weight fields for dispatch planning
            existing.TotalGrossWeight = dispatchPlanning.TotalGrossWeight;
            existing.TotalNetWeight = dispatchPlanning.TotalNetWeight;
            existing.UpdatedAt = DateTime.UtcNow;
    
            await _context.SaveChangesAsync();
            return existing;
        }
        
        public async Task<DispatchedRoll> CreateDispatchedRollAsync(DispatchedRoll dispatchedRoll)
        {
            _context.DispatchedRolls.Add(dispatchedRoll);
            await _context.SaveChangesAsync();
            return dispatchedRoll;
        }

        public async Task<IEnumerable<DispatchedRoll>> GetDispatchedRollsByPlanningIdAsync(int planningId)
        {
            return await _context.DispatchedRolls
                .Where(dr => dr.DispatchPlanningId == planningId)
                .ToListAsync();
        }

        public async Task<string> GenerateLoadingNoAsync()
        {
            // Format: LOAD{YY}{MM}{SERIAL}
            // YY = 2-digit year
            // MM = 2-digit month
            // SERIAL = 4-digit serial number

            // Get the latest record based on the highest loading number
            var lastRecord = await _context.DispatchPlannings
                .Where(dp => dp.LoadingNo.StartsWith("LOAD"))
                .OrderByDescending(dp => dp.LoadingNo)
                .FirstOrDefaultAsync();

            if (lastRecord != null && !string.IsNullOrEmpty(lastRecord.LoadingNo) && lastRecord.LoadingNo.Length >= 10)
            {
                // Extract the year, month, and serial from the last record
                var lastYear = lastRecord.LoadingNo.Substring(4, 2);  // Position 4-5
                var lastMonth = lastRecord.LoadingNo.Substring(6, 2); // Position 6-7
                var lastSerialStr = lastRecord.LoadingNo.Substring(8, 4); // Position 8-11

                if (int.TryParse(lastSerialStr, out int lastSerial))
                {
                    // Increment the serial number
                    int nextSerial = lastSerial + 1;
                    var prefix = $"LOAD{lastYear}{lastMonth}";
                    return $"{prefix}{nextSerial:D4}"; // Format as 4-digit number with leading zeros
                }
            }

            // If no records exist or parsing failed, start with the current date
            var now = DateTime.UtcNow;
            var currentYear = now.ToString("yy");
            var currentMonth = now.ToString("MM");
            var newPrefix = $"LOAD{currentYear}{currentMonth}";
            return $"{newPrefix}0001"; // Start with 0001
        }


        public async Task<string> GenerateDispatchOrderIdAsync()
        {
            // Format: DO{YY}{MM}{SERIAL}
            // YY = 2-digit year
            // MM = 2-digit month
            // SERIAL = 3-digit serial number

            // Get the latest record based on the highest dispatch order ID
            var lastRecord = await _context.DispatchPlannings
                .Where(dp => dp.DispatchOrderId.StartsWith("DO"))
                .OrderByDescending(dp => dp.DispatchOrderId)
                .FirstOrDefaultAsync();

            if (lastRecord != null && !string.IsNullOrEmpty(lastRecord.DispatchOrderId) && lastRecord.DispatchOrderId.Length >= 6)
            {
                // Extract the year, month, and serial from the last record
                var lastYear = lastRecord.DispatchOrderId.Substring(2, 2);  // Position 2-3
                var lastMonth = lastRecord.DispatchOrderId.Substring(4, 2); // Position 4-5
                var lastSerialStr = lastRecord.DispatchOrderId.Substring(6, 3); // Position 6-8

                if (int.TryParse(lastSerialStr, out int lastSerial))
                {
                    // Increment the serial number
                    int nextSerial = lastSerial + 1;
                    var prefix = $"DO{lastYear}{lastMonth}";
                    return $"{prefix}{nextSerial:D3}"; // Format as 3-digit number with leading zeros
                }
            }

            // If no records exist or parsing failed, start with the current date
            var now = DateTime.UtcNow;
            var currentYear = now.ToString("yy");
            var currentMonth = now.ToString("MM");
            var newPrefix = $"DO{currentYear}{currentMonth}";
            return $"{newPrefix}001"; // Start with 001
        }

    }
}