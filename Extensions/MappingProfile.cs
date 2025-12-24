using AutoMapper;
using AvyyanBackend.DTOs.Auth;
using AvyyanBackend.DTOs.User;
using AvyyanBackend.DTOs.Role;
using AvyyanBackend.DTOs.Machine;
using AvyyanBackend.DTOs.WebSocket;
using AvyyanBackend.DTOs.FabricStructure;
using AvyyanBackend.DTOs.Location;
using AvyyanBackend.DTOs.YarnType;
using AvyyanBackend.DTOs.SalesOrder;
using AvyyanBackend.Models;
using AvyyanBackend.DTOs.TapeColor;
using AvyyanBackend.DTOs.Shift;
using AvyyanBackend.DTOs.StorageCapture;
using AvyyanBackend.DTOs.DispatchPlanning;
using AvyyanBackend.DTOs.Transport;
using AvyyanBackend.DTOs.Courier;
using AvyyanBackend.DTOs.SlitLine; // Add this import

namespace AvyyanBackend.Extensions
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Auth Controller mappings
			CreateMap<User, AuthUserDto>();
			CreateMap<RegisterRequestDto, User>()
				.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
				.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => "User"));
			CreateMap<PageAccess, AuthPageAccessDto>();

			// User Controller mappings
			CreateMap<User, UserProfileResponseDto>();
			CreateMap<User, AdminUserResponseDto>();
			CreateMap<CreateUserRequestDto, User>()
				.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
			CreateMap<UpdateUserRequestDto, User>()
				.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.LastLoginAt, opt => opt.Ignore());
			CreateMap<UpdateUserProfileRequestDto, User>()
				.ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
				.ForMember(dest => dest.RoleName, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.Ignore());
			CreateMap<PageAccess, UserPageAccessDto>();

			// Add the missing mapping for UserPageAccessDto to AuthPageAccessDto
			CreateMap<UserPageAccessDto, AuthPageAccessDto>();

			// Role Controller mappings
			CreateMap<RoleMaster, RoleResponseDto>()
				.ForMember(dest => dest.PageAccesses, opt => opt.MapFrom(src => src.PageAccesses));
			CreateMap<CreateRoleRequestDto, RoleMaster>()
				.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Id, opt => opt.Ignore());
			CreateMap<UpdateRoleRequestDto, RoleMaster>()
				.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.PageAccesses, opt => opt.Ignore());
			CreateMap<PageAccess, PageAccessResponseDto>()
				.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
			CreateMap<PageAccess, RolePageAccessDto>();
			CreateMap<CreatePageAccessRequestDto, PageAccess>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Role, opt => opt.Ignore());
			CreateMap<UpdatePageAccessRequestDto, PageAccess>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Role, opt => opt.Ignore());

			// Machine Controller mappings
			CreateMap<MachineManager, MachineResponseDto>();
			CreateMap<CreateMachineRequestDto, MachineManager>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateMachineRequestDto, MachineManager>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			// Fabric Structure mappings
			CreateMap<FabricStructureMaster, FabricStructureResponseDto>();
			CreateMap<CreateFabricStructureRequestDto, FabricStructureMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateFabricStructureRequestDto, FabricStructureMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			// Complex mappings for user permissions
			CreateMap<User, UserPermissionsResponseDto>()
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.PageAccesses, opt => opt.Ignore()); // Will be mapped separately

			// Location mappings
			CreateMap<LocationMaster, LocationResponseDto>();
			CreateMap<CreateLocationRequestDto, LocationMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateLocationRequestDto, LocationMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			// Yarn Type mappings
			CreateMap<YarnTypeMaster, YarnTypeResponseDto>();
			CreateMap<CreateYarnTypeRequestDto, YarnTypeMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateYarnTypeRequestDto, YarnTypeMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			// Sales Order mappings
		

			// Sales Order Web mappings
			CreateMap<SalesOrderWeb, SalesOrderWebResponseDto>();
			CreateMap<SalesOrderItemWeb, SalesOrderItemWebResponseDto>();
			CreateMap<CreateSalesOrderWebRequestDto, SalesOrderWeb>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
				.ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.CreatedBy)); // For new items, createdBy becomes updatedBy initially
			CreateMap<CreateSalesOrderItemWebRequestDto, SalesOrderItemWeb>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.SalesOrderWebId, opt => opt.Ignore());
			CreateMap<UpdateSalesOrderWebRequestDto, SalesOrderWeb>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy));
			CreateMap<UpdateSalesOrderItemWebRequestDto, SalesOrderItemWeb>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.SalesOrderWebId, opt => opt.Ignore());

			// Tape Color mappings
			CreateMap<TapeColorMaster, TapeColorResponseDto>();
			CreateMap<CreateTapeColorRequestDto, TapeColorMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateTapeColorRequestDto, TapeColorMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			// Shift mappings
			CreateMap<ShiftMaster, ShiftResponseDto>();
			CreateMap<CreateShiftRequestDto, ShiftMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateShiftRequestDto, ShiftMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			// StorageCapture mappings
			CreateMap<StorageCapture, StorageCaptureResponseDto>();
			CreateMap<CreateStorageCaptureRequestDto, StorageCapture>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateStorageCaptureRequestDto, StorageCapture>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
				
			// Dispatch Planning mappings
			CreateMap<DispatchPlanning, DispatchPlanningDto>();
			CreateMap<CreateDispatchPlanningDto, DispatchPlanning>();
			CreateMap<UpdateDispatchPlanningDto, DispatchPlanning>();
			
			CreateMap<DispatchedRoll, DispatchedRollDto>();
			CreateMap<DispatchedRollDto, DispatchedRoll>();
			
			// Transport mappings
			CreateMap<TransportMaster, TransportResponseDto>();
			CreateMap<CreateTransportRequestDto, TransportMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateTransportRequestDto, TransportMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			// Courier mappings
			CreateMap<CourierMaster, CourierResponseDto>();
			CreateMap<CreateCourierRequestDto, CourierMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateCourierRequestDto, CourierMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

			// Slit Line mappings
			CreateMap<SlitLineMaster, SlitLineResponseDto>();
			CreateMap<CreateSlitLineRequestDto, SlitLineMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateSlitLineRequestDto, SlitLineMaster>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
		}
	}
}