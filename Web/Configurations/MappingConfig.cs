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
            // map to MovieDTO
            CreateMap<MovieDTO, MovieUpdateDTO>()
                .ForMember(dest => dest.GenreIds, opt => opt.MapFrom(src => src.Genres.Select(g => g.Id).ToList()));
            // Map from MovieDetailDTO to MovieUpdateDTO
            CreateMap<MovieDetailDTO, MovieUpdateDTO>()
                .ForMember(dest => dest.GenreIds, opt => opt.MapFrom(src => src.Genres.Select(g => g.Id).ToList()));
            // Map ConcessionDTo to ConcessionUpdateDTO
            CreateMap<ConcessionDTO, ConcessionUpdateDTO>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category != null ? src.Category.Id : 0));
            CreateMap<ProvinceDetailDTO, ProvinceUpdateDTO>();
            
            CreateMap<TheaterDetailDTO, TheaterUpdateDTO>()
                .ForMember(dest => dest.ProvinceId, opt => opt.MapFrom(src => src.Province.Id));

        }
    }
}