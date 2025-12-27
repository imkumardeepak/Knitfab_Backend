using AutoMapper;
using AvyyanBackend.DTOs.StorageCapture;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
	public class StorageCaptureService : IStorageCaptureService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRepository<StorageCapture> _storageCaptureRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<StorageCaptureService> _logger;

		public StorageCaptureService(
			IUnitOfWork unitOfWork,
			IRepository<StorageCapture> storageCaptureRepository,
			IMapper mapper,
			ILogger<StorageCaptureService> logger)
		{
			_unitOfWork = unitOfWork;
			_storageCaptureRepository = storageCaptureRepository;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<IEnumerable<StorageCaptureResponseDto>> GetAllStorageCapturesAsync()
		{
			_logger.LogDebug("Getting all storage captures");
			var storageCaptures = await _storageCaptureRepository.GetAllAsync();
			var orderstorgae = storageCaptures.OrderBy(a => a.LotNo).ThenBy(a => a.FGRollNo).ToList();
			_logger.LogInformation("Retrieved {StorageCaptureCount} storage captures", orderstorgae.Count());
			return _mapper.Map<IEnumerable<StorageCaptureResponseDto>>(orderstorgae);
		}

		public async Task<bool> GetStorageCaptureByLotNoAndFGRollNoAsync(string lotNo, string fgRollNo)
		{
			_logger.LogDebug("Getting storage capture by LotNo: {LotNo} and FGRollNo: {FGRollNo}", lotNo, fgRollNo);
			var storageCapture = await _storageCaptureRepository.FindAsync(m => m.LotNo == lotNo && m.FGRollNo == fgRollNo);
			if (!storageCapture.Any())
			{
				_logger.LogWarning("Storage capture not found for LotNo: {LotNo} and FGRollNo: {FGRollNo}", lotNo, fgRollNo);
				return false;
			}
			return true;
		}

		public async Task<StorageCaptureResponseDto> CreateStorageCaptureAsync(CreateStorageCaptureRequestDto createStorageCaptureDto)
		{
			_logger.LogDebug("Creating new storage capture for FGRollNo: {FGRollNo}", createStorageCaptureDto.FGRollNo);
			var storageCapture = _mapper.Map<StorageCapture>(createStorageCaptureDto);

			await _storageCaptureRepository.AddAsync(storageCapture);
			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("Created storage capture {StorageCaptureId} for FGRollNo: {FGRollNo}",
				storageCapture.Id, storageCapture.FGRollNo);
			return _mapper.Map<StorageCaptureResponseDto>(storageCapture);
		}

		public async Task<StorageCaptureResponseDto?> UpdateStorageCaptureAsync(int id, UpdateStorageCaptureRequestDto updateStorageCaptureDto)
		{
			_logger.LogDebug("Updating storage capture {StorageCaptureId}", id);

			var storageCapture = await _storageCaptureRepository.GetByIdAsync(id);
			if (storageCapture == null)
			{
				_logger.LogWarning("Storage capture {StorageCaptureId} not found for update", id);
				return null;
			}

			_mapper.Map(updateStorageCaptureDto, storageCapture);
			storageCapture.UpdatedAt = DateTime.UtcNow;

			_storageCaptureRepository.Update(storageCapture);
			await _unitOfWork.SaveChangesAsync();

			_logger.LogInformation("Updated storage capture {StorageCaptureId}", id);
			return _mapper.Map<StorageCaptureResponseDto>(storageCapture);
		}

		public async Task<bool> DeleteStorageCaptureAsync(int id)
		{
			_logger.LogDebug("Deleting storage capture {StorageCaptureId}", id);

			var storageCapture = await _storageCaptureRepository.GetByIdAsync(id);
			if (storageCapture == null)
			{
				_logger.LogWarning("Storage capture {StorageCaptureId} not found for deletion", id);
				return false;
			}
			_storageCaptureRepository.Remove(storageCapture);
			await _unitOfWork.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<StorageCaptureResponseDto>> SearchStorageCapturesAsync(StorageCaptureSearchRequestDto searchDto)
		{
			_logger.LogDebug("Searching storage captures with LotNo: {LotNo}, FGRollNo: {FGRollNo}",
				searchDto.LotNo, searchDto.FGRollNo);

			var storageCaptures = await _storageCaptureRepository.FindAsync(m =>
				(string.IsNullOrEmpty(searchDto.LotNo) || m.LotNo==searchDto.LotNo )&&

				(string.IsNullOrEmpty(searchDto.FGRollNo) || m.FGRollNo==searchDto.FGRollNo ))
				
				;

			// Order by LotNo and FGRollNo as requested
			var orderedStorageCaptures = storageCaptures.OrderBy(m => m.LotNo).ThenBy(m => m.FGRollNo).ToList();

			return _mapper.Map<IEnumerable<StorageCaptureResponseDto>>(orderedStorageCaptures);
		}
	}
}