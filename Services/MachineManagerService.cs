using AutoMapper;
using AvyyanBackend.DTOs.Machine;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class MachineManagerService : IMachineManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MachineManager> _machineRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MachineManagerService> _logger;

        public MachineManagerService(
            IUnitOfWork unitOfWork,
            IRepository<MachineManager> machineRepository,
            IMapper mapper,
            ILogger<MachineManagerService> logger)
        {
            _unitOfWork = unitOfWork;
            _machineRepository = machineRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<MachineResponseDto>> GetAllMachinesAsync()
        {
            _logger.LogDebug("Getting all machines");
            var machines = await _machineRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {MachineCount} machines", machines.Count());
            return _mapper.Map<IEnumerable<MachineResponseDto>>(machines);
        }

        public async Task<MachineResponseDto?> GetMachineByIdAsync(int id)
        {
            _logger.LogDebug("Getting machine by ID: {MachineId}", id);
            var machine = await _machineRepository.GetByIdAsync(id);
            if (machine == null)
            {
                _logger.LogWarning("Machine {MachineId} not found or inactive", id);
                return null;
            }
            return _mapper.Map<MachineResponseDto>(machine);
        }

        public async Task<MachineResponseDto> CreateMachineAsync(CreateMachineRequestDto createMachineDto)
        {
            _logger.LogDebug("Creating new machine: {MachineName}", createMachineDto.MachineName);

            // Check if machine name is unique
            var existingMachine = await _machineRepository.FirstOrDefaultAsync(m => m.MachineName == createMachineDto.MachineName);
            if (existingMachine != null)
            {
                throw new InvalidOperationException($"Machine with name '{createMachineDto.MachineName}' already exists");
            }

            var machine = _mapper.Map<MachineManager>(createMachineDto);
            await _machineRepository.AddAsync(machine);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created machine {MachineId} with name: {MachineName}", machine.Id, machine.MachineName);
            return _mapper.Map<MachineResponseDto>(machine);
        }

        public async Task<MachineResponseDto?> UpdateMachineAsync(int id, UpdateMachineRequestDto updateMachineDto)
        {
            _logger.LogDebug("Updating machine {MachineId}", id);

            var machine = await _machineRepository.GetByIdAsync(id);
            if (machine == null)
            {
                _logger.LogWarning("Machine {MachineId} not found for update", id);
                return null;
            }

            // Check if machine name is unique (excluding current machine)
            var existingMachine = await _machineRepository.FirstOrDefaultAsync(m => 
                m.MachineName == updateMachineDto.MachineName && m.Id != id);
            if (existingMachine != null)
            {
                throw new InvalidOperationException($"Machine with name '{updateMachineDto.MachineName}' already exists");
            }

            _mapper.Map(updateMachineDto, machine);
            machine.UpdatedAt = DateTime.UtcNow;

            _machineRepository.Update(machine);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated machine {MachineId}", id);
            return _mapper.Map<MachineResponseDto>(machine);
        }

        public async Task<bool> DeleteMachineAsync(int id)
        {
            _logger.LogDebug("Deleting machine {MachineId}", id);

            var machine = await _machineRepository.GetByIdAsync(id);
            if (machine == null)
            {
                _logger.LogWarning("Machine {MachineId} not found for deletion", id);
                return false;
            }
            _machineRepository.Remove(machine);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MachineResponseDto>> SearchMachinesAsync(MachineSearchRequestDto searchDto)
        {
            _logger.LogDebug("Searching machines with name: {MachineName}, dia: {Dia}", searchDto.MachineName, searchDto.Dia);

            var machines = await _machineRepository.FindAsync(m =>
                m.IsActive &&
                (string.IsNullOrEmpty(searchDto.MachineName) || m.MachineName.Contains(searchDto.MachineName)) &&
                (!searchDto.Dia.HasValue || m.Dia == searchDto.Dia.Value) &&
                (!searchDto.IsActive.HasValue || m.IsActive == searchDto.IsActive.Value));

            return _mapper.Map<IEnumerable<MachineResponseDto>>(machines);
        }

        public async Task<IEnumerable<MachineResponseDto>> CreateMultipleMachinesAsync(BulkCreateMachineRequestDto bulkCreateDto)
        {
            var machines = new List<MachineManager>();
            foreach (var dto in bulkCreateDto.Machines)
            {
                var machine = _mapper.Map<MachineManager>(dto);
                machines.Add(machine);
            }

            await _machineRepository.AddRangeAsync(machines);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created {MachineCount} machines in bulk", machines.Count);
            return _mapper.Map<IEnumerable<MachineResponseDto>>(machines);
        }

        public async Task<bool> IsMachineNameUniqueAsync(string machineName, int? excludeId = null)
        {
            var query = await _machineRepository.FindAsync(m => m.MachineName == machineName);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }
            return !query.Any();
        }
    }
}
