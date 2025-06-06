using API.Repositories.IRepositories;

namespace API.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        IMovieRepository Movie { get; }
        IGenreRepository Genre { get; }
        ITheaterRepository Theater { get; }
        IBookingRepository Booking { get; }
        IMovieGenreRepository MovieGenre { get; }
        IBookingDetailRepository BookingDetail { get; }
        IConcessionCategoryRepository ConcessionCategory { get; }
        IConcessionRepository Concession { get; }
        IConcessionOrderRepository ConcessionOrder { get; }
        IConcessionOrderDetailRepository ConcessionOrderDetail { get; }
        IPaymentRepository Payment { get; }
        IProvinceRepository Province { get; }
        IScreenRepository Screen { get; }
        ISeatRepository Seat { get; }
        ISeatTypeRepository SeatType { get; }
        IShowTimeRepository ShowTime { get; }
        
        Task SaveAsync();
    }
}
