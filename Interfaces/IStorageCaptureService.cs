using AvyyanBackend.DTOs.StorageCapture;

namespace AvyyanBackend.Interfaces
{
	public interface IStorageCaptureService
	{
		Task<IEnumerable<StorageCaptureResponseDto>> GetAllStorageCapturesAsync();
		Task<bool> GetStorageCaptureByLotNoAndFGRollNoAsync(string lotNo, string fgRollNo);
		Task<StorageCaptureResponseDto> CreateStorageCaptureAsync(CreateStorageCaptureRequestDto createStorageCaptureDto);
		Task<StorageCaptureResponseDto?> UpdateStorageCaptureAsync(int id, UpdateStorageCaptureRequestDto updateStorageCaptureDto);
		Task<bool> DeleteStorageCaptureAsync(int id);
		Task<IEnumerable<StorageCaptureResponseDto>> SearchStorageCapturesAsync(StorageCaptureSearchRequestDto searchDto);
	}
}