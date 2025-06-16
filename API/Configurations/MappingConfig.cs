using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using AutoMapper;
using Utility;

namespace API.Configurations;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        // ===================== MOVIE MAPPING =====================
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
                .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom((src, dest) =>
                    src.GenreIds.Select(id => new MovieGenre { GenreId = id, MovieId = dest.Id }).ToList()))
                .ForMember(dest => dest.ShowTimes, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now));

        // ===================== GENRE MAPPING =====================
        CreateMap<Genre, GenreDTO>();
        CreateMap<GenreCreateDTO, Genre>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now));
        CreateMap<GenreUpdateDTO, Genre>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now));

        // ===================== USER MAPPING =====================
        CreateMap<ApplicationUser, UserDTO>();
        CreateMap<UserUpdateDTO, ApplicationUser>();

        // ===================== BOOKING MAPPING =====================
        CreateMap<Booking, BookingDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.ApplicationUser != null ? src.ApplicationUser.Name : ""))
                .ForMember(dest => dest.BookingItems, opt => opt.MapFrom(src =>
                    src.BookingDetails))
                .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src =>
                    src.BookingStatus));
        // .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src =>
        //     src.ShowTime));
        CreateMap<BookingCreateDTO, Booking>()
                .ForMember(dest => dest.BookingCode, opt => opt.MapFrom(src =>
                    Guid.NewGuid().ToString()))
                .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src =>
                    Constant.Payment_Status_Pending))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src =>
                    src.BookingDetails.Sum(bd => bd.SeatPrice)))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
                    true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now));

        // ===================== BOOKING DETAIL MAPPING =====================
        CreateMap<BookingDetailItemDTO, BookingDetail>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now));
        CreateMap<BookingDetail, BookingDetailDTO>()
                .ForMember(dest => dest.SeatName, opt => opt.MapFrom(src =>
                    src.SeatName))
                .ForMember(dest => dest.SeatPrice, opt => opt.MapFrom(src =>
                    src.SeatPrice));
    }
}
