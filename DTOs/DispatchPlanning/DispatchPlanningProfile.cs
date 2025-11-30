using AutoMapper;
using AvyyanBackend.Models;
using AvyyanBackend.DTOs.DispatchPlanning;

namespace AvyyanBackend.DTOs.DispatchPlanning
{
    public class DispatchPlanningProfile : Profile
    {
        public DispatchPlanningProfile()
        {
            CreateMap<Models.DispatchPlanning, DispatchPlanningDto>()
                .ForMember(dest => dest.IsTransport, opt => opt.MapFrom(src => src.IsTransport))
                .ForMember(dest => dest.IsCourier, opt => opt.MapFrom(src => src.IsCourier))
                .ForMember(dest => dest.TransportId, opt => opt.MapFrom(src => src.TransportId))
                .ForMember(dest => dest.CourierId, opt => opt.MapFrom(src => src.CourierId));
                
            CreateMap<DispatchPlanningDto, Models.DispatchPlanning>()
                .ForMember(dest => dest.IsTransport, opt => opt.MapFrom(src => src.IsTransport))
                .ForMember(dest => dest.IsCourier, opt => opt.MapFrom(src => src.IsCourier))
                .ForMember(dest => dest.TransportId, opt => opt.MapFrom(src => src.TransportId))
                .ForMember(dest => dest.CourierId, opt => opt.MapFrom(src => src.CourierId));
                
            CreateMap<CreateDispatchPlanningDto, Models.DispatchPlanning>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LoadingNo, opt => opt.Ignore())
                .ForMember(dest => dest.DispatchOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsTransport, opt => opt.MapFrom(src => src.IsTransport))
                .ForMember(dest => dest.IsCourier, opt => opt.MapFrom(src => src.IsCourier))
                .ForMember(dest => dest.TransportId, opt => opt.MapFrom(src => src.TransportId))
                .ForMember(dest => dest.CourierId, opt => opt.MapFrom(src => src.CourierId));
                
            CreateMap<UpdateDispatchPlanningDto, Models.DispatchPlanning>()
                .ForMember(dest => dest.IsTransport, opt => opt.MapFrom(src => src.IsTransport))
                .ForMember(dest => dest.IsCourier, opt => opt.MapFrom(src => src.IsCourier))
                .ForMember(dest => dest.TransportId, opt => opt.MapFrom(src => src.TransportId))
                .ForMember(dest => dest.CourierId, opt => opt.MapFrom(src => src.CourierId));
            
            CreateMap<CreateDispatchPlanningRequestDto, Models.DispatchPlanning>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LoadingNo, opt => opt.Ignore())
                .ForMember(dest => dest.DispatchOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsTransport, opt => opt.MapFrom(src => src.IsTransport))
                .ForMember(dest => dest.IsCourier, opt => opt.MapFrom(src => src.IsCourier))
                .ForMember(dest => dest.TransportId, opt => opt.MapFrom(src => src.TransportId))
                .ForMember(dest => dest.CourierId, opt => opt.MapFrom(src => src.CourierId));
            
            CreateMap<Models.DispatchedRoll, DispatchedRollDto>();
            CreateMap<DispatchedRollDto, Models.DispatchedRoll>();
        }
    }
}