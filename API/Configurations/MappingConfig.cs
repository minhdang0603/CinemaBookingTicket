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

		CreateMap<MovieCreateDTO, Movie>()
			.ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
			.ForMember(dest => dest.ShowTimes, opt => opt.Ignore())
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
				DateTime.Now))
			.ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
				DateTime.Now))
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
		CreateMap<MovieUpdateDTO, Movie>()
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
				DateTime.Now))
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
				true));
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
				"PENDING"))
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
		CreateMap<Theater, TheaterDetailDTO>();
		CreateMap<TheaterCreateDTO, Theater>()
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
				DateTime.Now))
			.ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
				DateTime.Now));
		CreateMap<TheaterUpdateDTO, Theater>()
			.ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
				DateTime.Now));

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
		CreateMap<ShowTime, ShowTimeDTO>();

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

		// Mapping for ShowTimeSeatStatusDTO
		CreateMap<ShowTime, ShowTimeSeatStatusDTO>()
			.ForMember(dest => dest.ShowTimeId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.Movie.Id))
			.ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src =>
				src.Movie != null ? src.Movie.Title : string.Empty))
			.ForMember(dest => dest.ShowDate, opt => opt.MapFrom(src => src.ShowDate))
			.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
			.ForMember(dest => dest.ScreenName, opt => opt.MapFrom(src =>
				src.Screen != null ? src.Screen.Name : string.Empty))
			.ForMember(dest => dest.TheaterName, opt => opt.MapFrom(src =>
				src.Screen != null && src.Screen.Theater != null ? src.Screen.Theater.Name : string.Empty))
			.ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
			.ForMember(dest => dest.Seats, opt => opt.Ignore()); // Seats are populated separately

		// Mapping for SeatBookingStatusDTO
		CreateMap<Seat, SeatBookingStatusDTO>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.SeatRow, opt => opt.MapFrom(src => src.SeatRow))
			.ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
			.ForMember(dest => dest.SeatType, opt => opt.MapFrom(src => src.SeatType))
			.ForMember(dest => dest.Price, opt => opt.Ignore())  // Price is calculated in service
			.ForMember(dest => dest.IsBooked, opt => opt.Ignore()); // IsBooked is determined in service

		CreateMap<ShowTimeCreateDTO, ShowTime>()
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
					DateTime.Now))
				.ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
					DateTime.Now))
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
		CreateMap<ShowTimeUpdateDTO, ShowTime>()
				.ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
					DateTime.Now));

		// ===================== SEAT TYPE MAPPING =====================
		CreateMap<SeatType, SeatTypeDTO>()
			.ForMember(dest => dest.Color, opt => opt.MapFrom(src =>
				src.Color));

		// ====================== CONCESSION CATEGORY MAPPING =====================
		CreateMap<ConcessionCategory, ConcessionCategoryDTO>();
		CreateMap<ConcessionCategoryCreateDTO, ConcessionCategory>()
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
				true));
		CreateMap<ConcessionCategoryUpdateDTO, ConcessionCategory>();

		// ====================== CONCESSION Order DETAIL MAPPING =====================
		CreateMap<ConcessionOrderDetail, ConcessionOrderDetailDTO>()
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
				src.Concession != null ? src.Concession.Name : string.Empty))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
				src.Concession != null ? src.Concession.Description : string.Empty))
			.ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src =>
				src.Concession != null ? src.Concession.Price : 0))
			.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
				src.Concession != null ? src.Concession.ImageUrl : string.Empty));
		// ====================== CONCESSION MAPPING =====================
		CreateMap<Concession, ConcessionDTO>()
			.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
				src.Category));
		CreateMap<ConcessionCreateDTO, Concession>()
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
			.ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
			.ForMember(dest => dest.ConcessionOrderDetails, opt => opt.Ignore());
		CreateMap<ConcessionUpdateDTO, Concession>()
			.ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
			.ForMember(dest => dest.ConcessionOrderDetails, opt => opt.Ignore());

		// ====================== Province MAPPING =====================
		CreateMap<Province, ProvinceDTO>();
		CreateMap<ProvinceCreateDTO, Province>()
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
		CreateMap<ProvinceUpdateDTO, Province>();
		CreateMap<Province, ProvinceDetailDTO>();
	}
}
