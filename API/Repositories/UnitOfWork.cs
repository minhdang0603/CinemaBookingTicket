﻿using API.Data;
using API.Data.Models;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IMovieRepository Movie { get; private set; }
        public IGenreRepository Genre { get; private set; }
        public ITheaterRepository Theater { get; private set; }
        public IBookingRepository Booking { get; private set; }
        public IMovieGenreRepository MovieGenre { get; private set; }
        public IBookingDetailRepository BookingDetail { get; private set; }
        public IConcessionCategoryRepository ConcessionCategory { get; private set; }
        public IConcessionRepository Concession { get; private set; }
        public IConcessionOrderRepository ConcessionOrder { get; private set; }
        public IConcessionOrderDetailRepository ConcessionOrderDetail { get; private set; }
        public IPaymentRepository Payment { get; private set; }
        public IProvinceRepository Province { get; private set; }
        public IScreenRepository Screen { get; private set; }
        public ISeatRepository Seat { get; private set; }
        public ISeatTypeRepository SeatType { get; private set; }
        public IShowTimeRepository ShowTime { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Movie = new MovieRepository(_context);
            Genre = new GenreRepository(_context);
            Theater = new TheaterRepository(_context);
            Booking = new BookingRepository(_context);
            MovieGenre = new MovieGenreRepository(_context);
            BookingDetail = new BookingDetailRepository(_context);
            ConcessionCategory = new ConcessionCategoryRepository(_context);
            Concession = new ConcessionRepository(_context);
            ConcessionOrder = new ConcessionOrderRepository(_context);
            ConcessionOrderDetail = new ConcessionOrderDetailRepository(_context);
            Payment = new PaymentRepository(_context);
            Province = new ProvinceRepository(_context);
            Screen = new ScreenRepository(_context);
            Seat = new SeatRepository(_context);
            SeatType = new SeatTypeRepository(_context);
            ShowTime = new ShowTimeRepository(_context);
        }


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }
    }
}
