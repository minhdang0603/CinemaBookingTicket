using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using AutoMapper;

namespace API;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Movie, MovieDTO>().ReverseMap();
        CreateMap<MovieCreateDTO, Movie>();
        CreateMap<MovieUpdateDTO, Movie>();
        CreateMap<Genre, GenreDTO>().ReverseMap();
        CreateMap<GenreCreateDTO, Genre>();
        CreateMap<GenreUpdateDTO, Genre>();
    }
}
