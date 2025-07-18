using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data.Models;
using API.Data.DbInitializer;
using Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                    _context.Database.Migrate();
            }
            catch (Exception)
            {
            }

            if (!_roleManager.RoleExistsAsync(Constant.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Constant.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Constant.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Constant.Role_Admin)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "1234567890",
                }, "Admin@123").GetAwaiter().GetResult();

                ApplicationUser? user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, Constant.Role_Admin).GetAwaiter().GetResult();
                }
            }

            if (!_context.Movies.Any())
            {
                SeedSampleMovies();
            }

            return;
        }

        private void SeedSampleMovies()
        {

            if (!_context.Genres.Any())
            {
                var genres = new List<Genre>
        {
            new Genre { Name = "Hành động", Description = "Phim hành động kịch tính", IsActive = true },
            new Genre { Name = "Hài", Description = "Phim hài hước vui nhộn", IsActive = true },
            new Genre { Name = "Tình cảm", Description = "Phim tình cảm lãng mạn", IsActive = true },
            new Genre { Name = "Kinh dị", Description = "Phim kinh dị đáng sợ", IsActive = true },
            new Genre { Name = "Khoa học viễn tưởng", Description = "Phim khoa học viễn tưởng", IsActive = true },
            new Genre { Name = "Hoạt hình", Description = "Phim hoạt hình cho mọi lứa tuổi", IsActive = true }
        };

                _context.Genres.AddRange(genres);
                _context.SaveChanges();
            }

           
            var movies = new List<Movie>
{
    new Movie
    {
        Title = "Avengers: Endgame",
        Description = "Cuộc chiến cuối cùng của các siêu anh hùng Marvel",
        Director = "Anthony Russo, Joe Russo",
        Cast = "Robert Downey Jr., Chris Evans, Mark Ruffalo, Chris Hemsworth",
        Duration = 181,
        ReleaseDate = new DateOnly(2024, 6, 15),
        AgeRating = "PG-13",
        Status = Constant.Movie_Status_NowShowing, 
        PosterUrl = "https://m.media-amazon.com/images/M/MV5BMTc5MDE2ODcwNV5BMl5BanBnXkFtZTgwMzI2NzQ2NzM@._V1_.jpg",
        IsActive = true
    },
    new Movie
    {
        Title = "Spider-Man: No Way Home",
        Description = "Peter Parker phải đối mặt với các phản diện từ đa vũ trụ",
        Director = "Jon Watts",
        Cast = "Tom Holland, Zendaya, Benedict Cumberbatch",
        Duration = 148,
        ReleaseDate = new DateOnly(2024, 7, 1),
        AgeRating = "PG-13",
        Status = Constant.Movie_Status_NowShowing, 
        PosterUrl = "https://m.media-amazon.com/images/M/MV5BZWMyYzFjYTYtNTRjYi00OGExLWE2YzgtOGRmYjAxZTU3NzBiXkEyXkFqcGdeQXVyMzQ0MzA0NTM@._V1_.jpg",
        IsActive = true
    },
    new Movie
    {
        Title = "The Lion King",
        Description = "Câu chuyện về chú sư tử Simba và hành trình trở thành vua",
        Director = "Jon Favreau",
        Cast = "Donald Glover, Beyoncé, James Earl Jones",
        Duration = 118,
        ReleaseDate = new DateOnly(2024, 8, 10),
        AgeRating = "G",
        Status = Constant.Movie_Status_NowShowing, 
        PosterUrl = "https://m.media-amazon.com/images/M/MV5BMjIwMjE1Nzc4NV5BMl5BanBnXkFtZTgwNDg4OTA1NzM@._V1_.jpg",
        IsActive = true
    },
    new Movie
    {
        Title = "Fast & Furious 10",
        Description = "Gia đình Dom Toretto tiếp tục cuộc phiêu lưu tốc độ",
        Director = "Louis Leterrier",
        Cast = "Vin Diesel, Michelle Rodriguez, Tyrese Gibson",
        Duration = 141,
        ReleaseDate = new DateOnly(2024, 5, 20),
        AgeRating = "PG-13",
        Status = Constant.Movie_Status_NowShowing,
        PosterUrl = "https://m.media-amazon.com/images/M/MV5BNzVlYzk0NDMtNjgxYy00NmZhLWIwMDQtZDg5NjQ4YjkzMzQzXkEyXkFqcGdeQXVyODk4OTc3MTY@._V1_.jpg",
        IsActive = true
    },
    new Movie
    {
        Title = "The Conjuring 4",
        Description = "Ed và Lorraine Warren tiếp tục cuộc chiến với các thế lực siêu nhiên",
        Director = "Michael Chaves",
        Cast = "Patrick Wilson, Vera Farmiga",
        Duration = 112,
        ReleaseDate = new DateOnly(2024, 9, 5),
        AgeRating = "R",
        Status = Constant.Movie_Status_NowShowing, 
        PosterUrl = "https://m.media-amazon.com/images/M/MV5BMDU2NjIyOGYtMGFkYS00ZTk0LWJjMjMtNTdkZTdlZGUzNzg5XkEyXkFqcGdeQXVyMTkxNjUyNQ@@._V1_.jpg",
        IsActive = true
    },
    new Movie
    {
        Title = "Avatar 3",
        Description = "Phần tiếp theo của Avatar",
        Director = "James Cameron",
        Cast = "Sam Worthington, Zoe Saldana",
        Duration = 120,
        ReleaseDate = new DateOnly(2025, 12, 15),
        AgeRating = "PG-13",
        Status = Constant.Movie_Status_ComingSoon, 
        PosterUrl = "https://example.com/avatar3.jpg",
        IsActive = true
    }
};

            _context.Movies.AddRange(movies);
            _context.SaveChanges();

            var actionGenre = _context.Genres.First(g => g.Name == "Hành động");
            var sciFiGenre = _context.Genres.First(g => g.Name == "Khoa học viễn tưởng");
            var animationGenre = _context.Genres.First(g => g.Name == "Hoạt hình");
            var horrorGenre = _context.Genres.First(g => g.Name == "Kinh dị");

            var movieGenres = new List<MovieGenre>
    {
        new MovieGenre { MovieId = movies[0].Id, GenreId = actionGenre.Id },
        new MovieGenre { MovieId = movies[0].Id, GenreId = sciFiGenre.Id },
        new MovieGenre { MovieId = movies[1].Id, GenreId = actionGenre.Id },
        new MovieGenre { MovieId = movies[1].Id, GenreId = sciFiGenre.Id },
        new MovieGenre { MovieId = movies[2].Id, GenreId = animationGenre.Id },
        new MovieGenre { MovieId = movies[3].Id, GenreId = actionGenre.Id },
        new MovieGenre { MovieId = movies[4].Id, GenreId = horrorGenre.Id }
    };
            var currentTime = DateTime.Now;

            // 1. Seed SeatTypes first
            if (!_context.SeatTypes.Any())
            {
                var seatTypes = new List<SeatType>
        {
            new SeatType
            {
                Name = Constant.Seat_Type_Standard,
                Description = "Ghế tiêu chuẩn với giá cơ bản",
                PriceMultiplier = 1.0m,
                Color = "#6B7280", // Gray
                IsActive = true
            },
            new SeatType
            {
                Name = Constant.Seat_Type_Premium,
                Description = "Ghế cao cấp với không gian rộng rãi hơn",
                PriceMultiplier = 1.5m,
                Color = "#F59E0B", // Amber
                IsActive = true
            },
            new SeatType
            {
                Name = Constant.Seat_Type_VIP,
                Description = "Ghế VIP sang trọng với dịch vụ đặc biệt",
                PriceMultiplier = 2.0m,
                Color = "#DC2626", // Red
                IsActive = true
            }
        };

                _context.SeatTypes.AddRange(seatTypes);
                _context.SaveChanges();
            }

            // 2. Seed Provinces 
            if (!_context.Provinces.Any())
            {
                var provinces = new List<Province>
        {
            new Province { Name = "TP. Hồ Chí Minh", Code = "HCM", IsActive = true },
            new Province { Name = "Hà Nội", Code = "HN", IsActive = true },
            new Province { Name = "Đà Nẵng", Code = "DN", IsActive = true },
            new Province { Name = "Cần Thơ", Code = "CT", IsActive = true },
            new Province { Name = "Hải Phòng", Code = "HP", IsActive = true }
        };

                _context.Provinces.AddRange(provinces);
                _context.SaveChanges();
            }

            // 3. Seed Theaters 
            if (!_context.Theaters.Any())
            {
                var provinces = _context.Provinces.ToList();
                var theaters = new List<Theater>();

                foreach (var province in provinces)
                {
                    theaters.AddRange(new List<Theater>
            {
                new Theater
                {
                    Name = $"CGV {province.Name}",
                    Address = $"123 Đường Nguyễn Huệ, {province.Name}",
                    OpeningTime = new TimeOnly(9, 0), // 9:00 AM
                    ClosingTime = new TimeOnly(23, 30), // 11:30 PM
                    Description = $"Rạp chiếu phim CGV tại {province.Name}",
                    ProvinceId = province.Id,
                    IsActive = true,
                    CreatedAt = currentTime,
                    LastUpdatedAt = currentTime
                },
                new Theater
                {
                    Name = $"Lotte Cinema {province.Name}",
                    Address = $"456 Đường Trần Hưng Đạo, {province.Name}",
                    OpeningTime = new TimeOnly(8, 30), // 8:30 AM
                    ClosingTime = new TimeOnly(23, 45), // 11:45 PM
                    Description = $"Rạp chiếu phim Lotte Cinema tại {province.Name}",
                    ProvinceId = province.Id,
                    IsActive = true,
                    CreatedAt = currentTime,
                    LastUpdatedAt = currentTime
                }
            });
                }

                _context.Theaters.AddRange(theaters);
                _context.SaveChanges();
            }

            // 4. Seed Screens 
            if (!_context.Screens.Any())
            {
                var theaters = _context.Theaters.ToList();
                var screens = new List<Screen>();

                foreach (var theater in theaters)
                {
                    screens.AddRange(new List<Screen>
            {
                new Screen
                {
                    Name = "Screen 1",
                    TheaterId = theater.Id,
                    Rows = 10, // A-J
                    SeatsPerRow = 15, // 1-15
                    IsActive = true,
                    CreatedAt = currentTime,
                    LastUpdatedAt = currentTime
                },
                new Screen
                {
                    Name = "Screen 2 - IMAX",
                    TheaterId = theater.Id,
                    Rows = 12, // A-L
                    SeatsPerRow = 18, // 1-18
                    IsActive = true,
                    CreatedAt = currentTime,
                    LastUpdatedAt = currentTime
                }
            });
                }

                _context.Screens.AddRange(screens);
                _context.SaveChanges();
            }

            // 5. Seed Seats cho mỗi Screen
            if (!_context.Seats.Any())
            {
                var screens = _context.Screens.ToList();
                var seatTypes = _context.SeatTypes.ToList();
                var standardType = seatTypes.First(st => st.Name == Constant.Seat_Type_Standard);
                var premiumType = seatTypes.First(st => st.Name == Constant.Seat_Type_Premium);
                var vipType = seatTypes.First(st => st.Name == Constant.Seat_Type_VIP);

                var seats = new List<Seat>();

                foreach (var screen in screens)
                {
                    for (int row = 0; row < screen.Rows; row++)
                    {
                        char rowLetter = (char)('A' + row);

                        for (int seatNum = 1; seatNum <= screen.SeatsPerRow; seatNum++)
                        {
                            // Determine seat type based on position
                            SeatType seatType;
                            if (row >= screen.Rows - 2) // Last 2 rows = VIP
                                seatType = vipType;
                            else if (row >= screen.Rows - 5) // Middle rows = Premium
                                seatType = premiumType;
                            else // Front rows = Standard
                                seatType = standardType;

                            seats.Add(new Seat
                            {
                                ScreenId = screen.Id,
                                SeatRow = rowLetter.ToString(),
                                SeatNumber = seatNum,
                                SeatTypeId = seatType.Id,
                                IsActive = true,
                                CreatedAt = currentTime,
                                LastUpdatedAt = currentTime
                            });
                        }
                    }
                }

                _context.Seats.AddRange(seats);
                _context.SaveChanges();
            }

            if (!_context.ShowTimes.Any())
            {
                movies = _context.Movies.Where(m => m.IsActive && m.Status == Constant.Movie_Status_NowShowing).ToList();
                var screens = _context.Screens.Where(s => s.IsActive).ToList();
                var showTimes = new List<ShowTime>();

                var timeSlots = new List<TimeOnly>
        {
            new TimeOnly(9, 0),   
            new TimeOnly(12, 0),  
            new TimeOnly(15, 0),  
            new TimeOnly(18, 0),  
            new TimeOnly(21, 0)   
        };

                // Generate showtimes for next 7 days
                for (int dayOffset = 0; dayOffset < 7; dayOffset++)
                {
                    var showDate = DateOnly.FromDateTime(DateTime.Now.AddDays(dayOffset));

                    foreach (var screen in screens)
                    {
                        // Each screen shows 2-3 different movies per day
                        var dailyMovies = movies.Take(3).ToList();
                        var timeSlotIndex = 0;

                        foreach (var movie in dailyMovies)
                        {
                            if (timeSlotIndex < timeSlots.Count)
                            {
                                var startTime = timeSlots[timeSlotIndex];
                                var endTime = startTime.AddMinutes(movie.Duration + 30); // +30 for ads/cleaning

                                // Base price varies by time slot
                                decimal basePrice = timeSlotIndex switch
                                {
                                    0 or 1 => 80000, // Morning/Afternoon
                                    2 => 100000,     // Evening
                                    3 or 4 => 120000, // Night
                                    _ => 100000
                                };

                                showTimes.Add(new ShowTime
                                {
                                    MovieId = movie.Id,
                                    ScreenId = screen.Id,
                                    ShowDate = showDate,
                                    StartTime = startTime,
                                    EndTime = endTime,
                                    BasePrice = basePrice,
                                    IsActive = true,
                                    CreatedAt = currentTime,
                                    LastUpdatedAt = currentTime
                                });

                                timeSlotIndex += 2; // Skip one time slot between movies
                            }
                        }
                    }
                }

                _context.ShowTimes.AddRange(showTimes);
                _context.SaveChanges();
            }

            _context.MovieGenres.AddRange(movieGenres);
            _context.SaveChanges();
        }
    }
}
