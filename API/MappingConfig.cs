using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using AutoMapper;
using Utility;

namespace API;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Movie, MovieDTO>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                    src.MovieGenres != null ? src.MovieGenres.Select(mg => mg.Genre) : null));
                // .ForMember(dest => dest.ShowTimes, opt => opt.Ignore());
        CreateMap<MovieCreateDTO, Movie>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                    Constant.Movie_Status_ComingSoon))
                .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
                .ForMember(dest => dest.ShowTimes, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => 
                    DateTime.Now))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => 
                    DateTime.Now));
        CreateMap<MovieUpdateDTO, Movie>()
                .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
                .ForMember(dest => dest.ShowTimes, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => 
                    DateTime.Now));
        CreateMap<Genre, GenreDTO>();
        CreateMap<GenreCreateDTO, Genre>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => 
                    DateTime.Now))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => 
                    DateTime.Now));
        CreateMap<GenreUpdateDTO, Genre>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => 
                    DateTime.Now));
        CreateMap<ShowTime, ShowTimeDTO>();
    }
}
