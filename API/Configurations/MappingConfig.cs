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

        // Add MovieLiteDTO mapping for use in ShowTimeDTO
        CreateMap<Movie, MovieLiteDTO>();

        CreateMap<Movie, MovieDetailDTO>()
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                src.MovieGenres != null ? src.MovieGenres.Select(mg => mg.Genre) : null))
            .ForMember(dest => dest.ShowTimes, opt => opt.MapFrom(src =>
                src.ShowTimes != null ? src.ShowTimes.Select(st => new ShowTimeLiteDTO
                {
                    Id = st.Id,
                    ShowDate = st.ShowDate,
                    StartTime = st.StartTime,
                    EndTime = st.EndTime,
                    BasePrice = st.BasePrice,
                    Screen = new ScreenDTO { Id = st.Screen.Id, Name = st.Screen.Name }
                }) : null));

        CreateMap<MovieCreateDTO, Movie>()
            .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
            .ForMember(dest => dest.ShowTimes, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                DateTime.Now))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                DateTime.Now)); CreateMap<MovieUpdateDTO, Movie>()
            .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom((src, dest) =>
                src.GenreIds != null ? src.GenreIds.Select(id => new MovieGenre { GenreId = id, MovieId = dest.Id }).ToList() : new List<MovieGenre>()))
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
            .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src =>
                src.BookingStatus))
            .ForMember(dest => dest.BookingItems, opt => opt.MapFrom(src =>
                src.BookingDetails))
            .ForMember(dest => dest.ShowTime, opt => opt.MapFrom(src =>
                src.ShowTime));
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

        // ===================== THEATER MAPPING =====================
        CreateMap<Theater, TheaterDTO>();
        CreateMap<Theater, TheaterDetailDTO>().ForMember(dest => dest.Screens, opt => opt.MapFrom(src =>
                src.Screens != null ? src.Screens.Select(s => new ScreenLiteDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Rows = s.Rows,
                    SeatsPerRow = s.SeatsPerRow
                }) : null));
        CreateMap<TheaterCreateDTO, Theater>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                DateTime.Now))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                DateTime.Now))
            .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src =>
                TimeOnly.FromDateTime(src.OpeningTime!.Value)))
            .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src =>
                TimeOnly.FromDateTime(src.ClosingTime!.Value)));
        CreateMap<TheaterUpdateDTO, Theater>()
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                DateTime.Now))
            .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src =>
                TimeOnly.FromDateTime(src.OpeningTime!.Value)))
            .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src =>
                TimeOnly.FromDateTime(src.ClosingTime!.Value)));

        // ===================== SCREEN MAPPING =====================

        CreateMap<Screen, ScreenDTO>();
        CreateMap<Screen, ScreenLiteDTO>();
        CreateMap<Screen, ScreenDetailDTO>();

        CreateMap<ScreenCreateDTO, Screen>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                DateTime.Now))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                DateTime.Now))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
                true));
        CreateMap<ScreenUpdateDTO, Screen>()
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                DateTime.Now));

        // ===================== SEAT MAPPING =====================
        CreateMap<Seat, SeatDTO>()
            .ForMember(dest => dest.SeatType, opt => opt.MapFrom(src => 
                src.SeatType));

        CreateMap<SeatUpdateDTO, Seat>()
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                DateTime.Now));

        // ===================== SHOWTIME MAPPING =====================
        CreateMap<ShowTime, ShowTimeDTO>()
            .ForMember(dest => dest.Movie, opt => opt.MapFrom(src =>
                src.Movie))
            .ForMember(dest => dest.Screen, opt => opt.MapFrom(src =>
                src.Screen));

        CreateMap<ShowTime, ShowTimeLiteDTO>()
            .ForMember(dest => dest.Screen, opt => opt.MapFrom(src =>
                src.Screen));

        // Summary DTO for use in booking - avoids circular references
        CreateMap<ShowTime, ShowTimeSummaryDTO>()
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src =>
                src.Movie != null ? src.Movie.Title : string.Empty))
            .ForMember(dest => dest.TheaterName, opt => opt.MapFrom(src =>
                src.Screen != null && src.Screen.Theater != null ? src.Screen.Theater.Name : string.Empty))
            .ForMember(dest => dest.ScreenName, opt => opt.MapFrom(src =>
                src.Screen != null ? src.Screen.Name : string.Empty));

        CreateMap<ShowTimeCreateDTO, ShowTime>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now));
        CreateMap<ShowTimeUpdateDTO, ShowTime>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                    DateTime.Now));

        // ===================== SEAT TYPE MAPPING =====================
        CreateMap<SeatType, SeatTypeDTO>()
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src =>
                src.Color));
    }
}
