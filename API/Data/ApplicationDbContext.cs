using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using API.Data.Models;

namespace API.Data.Models;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Concession> Concessions { get; set; }

    public virtual DbSet<ConcessionCategory> ConcessionCategories { get; set; }

    public virtual DbSet<ConcessionOrder> ConcessionOrders { get; set; }

    public virtual DbSet<ConcessionOrderDetail> ConcessionOrderDetails { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieGenre> MovieGenres { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Screen> Screens { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatType> SeatTypes { get; set; }

    public virtual DbSet<ShowTime> ShowTimes { get; set; }

    public virtual DbSet<Theater> Theaters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(e => e.BookingCode, "IX_Bookings_BookingCode").IsUnique();

            entity.Property(e => e.BookingCode).HasMaxLength(20);
            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ShowTime).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ShowTimeId)
                .HasConstraintName("FK_Bookings_ShowTimes");
        });

    modelBuilder.Entity<Booking>()
        .HasOne(b => b.Payment)
        .WithOne(p => p.Booking)
        .HasForeignKey<Payment>(p => p.BookingId)
        .HasConstraintName("FK_Payments_Bookings")
        .OnDelete(DeleteBehavior.Cascade);


    modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SeatName).HasMaxLength(10);
            entity.Property(e => e.SeatPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingDetails_Bookings");

            entity.HasOne(d => d.Seat).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingDetails_Seats");

            entity.HasIndex(e => new { e.BookingId, e.SeatId }, "UK_BookingDetails_Booking_Seat").IsUnique();
        });

        modelBuilder.Entity<Concession>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Concessions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Concessions_Categories");
        });

        modelBuilder.Entity<ConcessionCategory>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ConcessionOrder>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.ConcessionOrders)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_ConcessionOrders_Bookings");
        });

        modelBuilder.Entity<ConcessionOrderDetail>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Concession).WithMany(p => p.ConcessionOrderDetails)
                .HasForeignKey(d => d.ConcessionId)
                .HasConstraintName("FK_ConcessionOrderDetails_Concessions");

            entity.HasOne(d => d.ConcessionOrder).WithMany(p => p.ConcessionOrderDetails)
                .HasForeignKey(d => d.ConcessionOrderId)
                .HasConstraintName("FK_ConcessionOrderDetails_Orders");

            entity.HasIndex(e => new { e.ConcessionOrderId, e.ConcessionId }, "UK_ConcessionOrderDetails_Order_Concession").IsUnique();
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasIndex(e => e.Name, "UK_Genres_Name").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasIndex(e => new { e.Status, e.IsActive }, "IX_Movies_Status");

            entity.Property(e => e.AgeRating).HasMaxLength(10);
            entity.Property(e => e.BackgroundUrl).HasMaxLength(500);
            entity.Property(e => e.Cast).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Director).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PosterUrl).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("coming soon");
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.TrailerUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<MovieGenre>(entity =>
        {
            entity.HasIndex(e => new { e.MovieId, e.GenreId }, "UK_MovieGenres_Movie_Genre").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderCode).HasMaxLength(100);
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentGateway).HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Screen>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Theater).WithMany(p => p.Screens)
                .HasForeignKey(d => d.TheaterId)
                .HasConstraintName("FK_Screens_Theaters");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasIndex(e => new { e.ScreenId, e.SeatRow, e.SeatNumber }, "UK_Seats_Screen_Row_Number").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SeatRow).HasMaxLength(5);

            entity.HasOne(d => d.Screen).WithMany(p => p.Seats)
                .HasForeignKey(d => d.ScreenId)
                .HasConstraintName("FK_Seats_Screens");

            entity.HasOne(d => d.SeatType).WithMany(p => p.Seats)
                .HasForeignKey(d => d.SeatTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seats_SeatTypes");
        });

        modelBuilder.Entity<SeatType>(entity =>
        {
            entity.Property(e => e.Color).HasMaxLength(7);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PriceMultiplier)
                .HasDefaultValue(1.00m)
                .HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<ShowTime>(entity =>
        {
            entity.HasIndex(e => new { e.IsActive, e.ShowDate, e.StartTime }, "IX_ShowTimes_Active_Date");

            entity.HasIndex(e => new { e.MovieId, e.ScreenId }, "IX_ShowTimes_Movie_Screen");

            entity.HasIndex(e => e.ShowDate, "IX_ShowTimes_ShowDate");

            entity.HasIndex(e => new { e.MovieId, e.ScreenId, e.ShowDate, e.StartTime })
              .HasDatabaseName("UK_ShowTimes_Movie_Screen_Date_Time")
              .IsUnique();

            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Movie).WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK_ShowTimes_Movies");

            entity.HasOne(d => d.Screen).WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.ScreenId)
                .HasConstraintName("FK_ShowTimes_Screens");
        });

        modelBuilder.Entity<Theater>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Province).WithMany(p => p.Theaters)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Theaters_Provinces");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
