using AvyyanBackend.DTOs.DispatchPlanning;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using AutoMapper;

namespace AvyyanBackend.Services
{
    public class DispatchPlanningService
    {
        private readonly IDispatchPlanningRepository _repository;
        private readonly IMapper _mapper;

        public DispatchPlanningService(IDispatchPlanningRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DispatchPlanningDto>> GetAllAsync()
        {
            var dispatchPlannings = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<DispatchPlanningDto>>(dispatchPlannings);
        }

        public async Task<DispatchPlanningDto?> GetByIdAsync(int id)
        {
            var dispatchPlanning = await _repository.GetByIdAsync(id);
            return dispatchPlanning == null ? null : _mapper.Map<DispatchPlanningDto>(dispatchPlanning);
        }

        public async Task<DispatchPlanningDto> CreateAsync(CreateDispatchPlanningDto createDto)
        {
            // Generate LoadingNo
            var loadingNo = await _repository.GenerateLoadingNoAsync();
            
            // Generate DispatchOrderId
            var dispatchOrderId = await _repository.GenerateDispatchOrderIdAsync();
            
            var dispatchPlanning = _mapper.Map<DispatchPlanning>(createDto);
            dispatchPlanning.LoadingNo = loadingNo;
            dispatchPlanning.DispatchOrderId = dispatchOrderId;
            dispatchPlanning.CreatedAt = DateTime.UtcNow;
            dispatchPlanning.IsActive = true;
            
            // Check if required rolls match dispatched rolls to determine status
            dispatchPlanning.IsFullyDispatched = false;
            
            var created = await _repository.CreateAsync(dispatchPlanning);
            return _mapper.Map<DispatchPlanningDto>(created);
        }

        public async Task<DispatchPlanningDto> UpdateAsync(int id, UpdateDispatchPlanningDto updateDto)
        {
            var dispatchPlanning = _mapper.Map<DispatchPlanning>(updateDto);
            var updated = await _repository.UpdateAsync(id, dispatchPlanning);
            
            // Check if required rolls match dispatched rolls to determine status
            updated.IsFullyDispatched = updated.TotalRequiredRolls <= updated.TotalDispatchedRolls;
            
            return _mapper.Map<DispatchPlanningDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DispatchedRollDto>> GetDispatchedRollsByPlanningIdAsync(int planningId)
        {
            var dispatchedRolls = await _repository.GetDispatchedRollsByPlanningIdAsync(planningId);
            return _mapper.Map<IEnumerable<DispatchedRollDto>>(dispatchedRolls);
        }

        public async Task<DispatchedRollDto> CreateDispatchedRollAsync(DispatchedRollDto dto)
        {
            var dispatchedRoll = _mapper.Map<DispatchedRoll>(dto);
            dispatchedRoll.CreatedAt = DateTime.UtcNow;
            dispatchedRoll.IsActive = true;
            
            var created = await _repository.CreateDispatchedRollAsync(dispatchedRoll);
            return _mapper.Map<DispatchedRollDto>(created);
        }

        // New method to create dispatch planning from dispatch details
        public async Task<DispatchPlanningDto> CreateFromDispatchDetailsAsync(
            string lotNo,
            int salesOrderId,
            int salesOrderItemId,
            string customerName,
            string tape,
            decimal totalRequiredRolls,
            decimal totalReadyRolls,
            decimal totalDispatchedRolls,
            string vehicleNo,
            string driverName,
            string license,
            string mobileNumber,
            string remarks,
            string dispatchOrderId = null)
        {
            // Generate DispatchOrderId if not provided
            var finalDispatchOrderId = dispatchOrderId ?? await _repository.GenerateDispatchOrderIdAsync();
            
            var createDto = new CreateDispatchPlanningDto
            {
                LotNo = lotNo,
                SalesOrderId = salesOrderId,
                SalesOrderItemId = salesOrderItemId,
                CustomerName = customerName,
                Tape = tape,
                TotalRequiredRolls = totalRequiredRolls,
                TotalReadyRolls = totalReadyRolls,
                TotalDispatchedRolls = totalDispatchedRolls,
                // Only mark as fully dispatched if required rolls match dispatched rolls
                IsFullyDispatched = totalRequiredRolls <= totalDispatchedRolls,
                DispatchStartDate = DateTime.UtcNow,
                DispatchEndDate = DateTime.UtcNow,
                VehicleNo = vehicleNo,
                DriverName = driverName,
                License = license,
                MobileNumber = mobileNumber,
                Remarks = remarks
            };

            // Create the dispatch planning record
            var dispatchPlanning = _mapper.Map<DispatchPlanning>(createDto);
            dispatchPlanning.DispatchOrderId = finalDispatchOrderId;
            dispatchPlanning.CreatedAt = DateTime.UtcNow;
            dispatchPlanning.IsActive = true;
            
            // Generate LoadingNo
            dispatchPlanning.LoadingNo = await _repository.GenerateLoadingNoAsync();
            
            // Check if required rolls match dispatched rolls to determine status
            dispatchPlanning.IsFullyDispatched = dispatchPlanning.TotalRequiredRolls <= dispatchPlanning.TotalDispatchedRolls;
            
            var created = await _repository.CreateAsync(dispatchPlanning);
            return _mapper.Map<DispatchPlanningDto>(created);
        }
        
        // New method to create multiple dispatch planning records with sequential loading numbers
        public async Task<IEnumerable<DispatchPlanningDto>> CreateBatchAsync(
            IEnumerable<CreateDispatchPlanningRequestDto> createDtos,
            string dispatchOrderId = null)
        {
            var results = new List<DispatchPlanningDto>();
            
            // Generate a single dispatch order ID for all records if not provided
            var finalDispatchOrderId = dispatchOrderId ?? await GenerateDispatchOrderIdAsync();
            
            // Generate the first loading number
            var firstLoadingNo = await _repository.GenerateLoadingNoAsync();
            
            // Parse the first loading number to get the prefix and serial
            string prefix = "";
            int baseSerial = 1;
            
            if (firstLoadingNo.Length >= 8 && firstLoadingNo.StartsWith("LOAD"))
            {
                prefix = firstLoadingNo.Substring(0, 8); // LOADYYMM
                var serialStr = firstLoadingNo.Substring(8); // The serial part
                if (int.TryParse(serialStr, out int parsedSerial))
                {
                    baseSerial = parsedSerial;
                }
            }
            else
            {
                // Fallback to current date if parsing fails
                var now = DateTime.UtcNow;
                var year = now.ToString("yy");
                var month = now.ToString("MM");
                prefix = $"LOAD{year}{month}";
                baseSerial = 1;
            }
            
            int index = 0;
            foreach (var createDto in createDtos)
            {
                // Create each dispatch planning record with the same dispatch order ID
                var dispatchPlanning = _mapper.Map<DispatchPlanning>(createDto);
                dispatchPlanning.DispatchOrderId = finalDispatchOrderId;
                dispatchPlanning.CreatedAt = DateTime.UtcNow;
                dispatchPlanning.IsActive = true;
                
                // Generate sequential loading numbers
                var loadingNo = $"{prefix}{(baseSerial + index):D4}";
                dispatchPlanning.LoadingNo = loadingNo;
                
                // Check if required rolls match dispatched rolls to determine status
                dispatchPlanning.IsFullyDispatched = dispatchPlanning.TotalRequiredRolls <= dispatchPlanning.TotalDispatchedRolls;
                
                var created = await _repository.CreateAsync(dispatchPlanning);
                results.Add(_mapper.Map<DispatchPlanningDto>(created));
                
                index++;
            }
            
            return results;
        }
        
        // New method to update dispatch planning status based on roll counts
        public async Task<DispatchPlanningDto> UpdateDispatchStatusAsync(int id, decimal totalDispatchedRolls)
        {
            var dispatchPlanning = await _repository.GetByIdAsync(id);
            if (dispatchPlanning == null)
                throw new Exception("DispatchPlanning not found");

            // Update the dispatched rolls count
            dispatchPlanning.TotalDispatchedRolls = totalDispatchedRolls;
            
            // Only mark as fully dispatched if required rolls match dispatched rolls
            dispatchPlanning.IsFullyDispatched = dispatchPlanning.TotalRequiredRolls <= dispatchPlanning.TotalDispatchedRolls;
            dispatchPlanning.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(id, dispatchPlanning);
            return _mapper.Map<DispatchPlanningDto>(updated);
        }
        
        // New method to generate dispatch order ID
        public async Task<string> GenerateDispatchOrderIdAsync()
        {
            return await _repository.GenerateDispatchOrderIdAsync();
        }

        // New method to get dispatched rolls ordered by lotNo and fgRoll sequence
        public async Task<IEnumerable<DispatchedRollDto>> GetOrderedDispatchedRollsByDispatchOrderIdAsync(string dispatchOrderId)
        {
            // First get all dispatch planning records for this dispatch order ID
            var dispatchPlannings = await _repository.GetAllAsync();
            var filteredPlannings = dispatchPlannings.Where(dp => dp.DispatchOrderId == dispatchOrderId).ToList();
            
            // Get all dispatched rolls for these planning records
            var allDispatchedRolls = new List<DispatchedRoll>();
            foreach (var planning in filteredPlannings)
            {
                var rolls = await _repository.GetDispatchedRollsByPlanningIdAsync(planning.Id);
                allDispatchedRolls.AddRange(rolls);
            }
            
            // Order by LotNo and FGRollNo
            var orderedRolls = allDispatchedRolls
                .OrderBy(dr => dr.LotNo)
                .ThenBy(dr => dr.FGRollNo)
                .ToList();
            
            return _mapper.Map<IEnumerable<DispatchedRollDto>>(orderedRolls);
        }
    }
}