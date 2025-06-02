using API.Repositories.IRepositories;

namespace CinemaBookingTicket_API.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        IMovieRepository Movie { get; }
        IGenreRepository Genre { get; }
        ITheaterRepository Theater { get; }
        IScreenRepository Screen { get; }
        ISeatRepository Seat { get; }
        ISeatTypeRepository SeatType { get; }
        IProvinceRepository Province { get; }
        IShowTimeRepository ShowTime { get; }
        IBookingRepository Booking { get; }
        IBookingDetailRepository BookingDetail { get; }
        IPaymentRepository Payment { get; }
        IConcessionRepository Concession { get; }
        IConcessionCategoryRepository ConcessionCategory { get; }
        IConcessionOrderRepository ConcessionOrder { get; }
        IConcessionOrderDetailRepository ConcessionOrderDetail { get; }
        IMovieGenreRepository MovieGenre { get; }
        Task SaveAsync();
    }
}
