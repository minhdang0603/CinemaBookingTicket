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

            CreateMap<ShowTimeDTO, ShowTimeUpdateDTO>()
                .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.Movie.Id))
                .ForMember(dest => dest.ScreenId, opt => opt.MapFrom(src => src.Screen.Id));
            CreateMap<ProvinceDetailDTO, ProvinceUpdateDTO>();
            
            CreateMap<TheaterDetailDTO, TheaterUpdateDTO>()
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.Province.Id));

        }
    }
}