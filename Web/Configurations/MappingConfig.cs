using AutoMapper;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Configurations
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // Map ScreenDetailDTO to ScreenUpdateDTO
            CreateMap<ScreenDetailDTO, ScreenUpdateDTO>()
                .ForMember(dest => dest.Seats, opt => opt.MapFrom(src => src.Seats));

            CreateMap<SeatDTO, SeatUpdateDTO>();

            CreateMap<ProvinceDetailDTO, ProvinceUpdateDTO>();
            
            CreateMap<SeatDTO, SeatUpdateDTO>();
        }
    }
}