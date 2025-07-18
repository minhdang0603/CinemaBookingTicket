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

            // create roles if they are not created
            if (!_roleManager.RoleExistsAsync(Constant.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Constant.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Constant.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Constant.Role_Admin)).GetAwaiter().GetResult();

                // create admin user if it does not exist
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

            // Seed sample movies if no movies exist
            if (!_context.Movies.Any())
            {
                SeedSampleMovies();
            }

            return;
        }

        private void SeedSampleMovies()
        {
            // Seed genres first if they don't exist
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

            // Add sample movies
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
            Status = "NowShowing",
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
            Status = "NowShowing",
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
            Status = "NowShowing",
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
            Status = "NowShowing",
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
            Status = "NowShowing",
            PosterUrl = "https://m.media-amazon.com/images/M/MV5BMDU2NjIyOGYtMGFkYS00ZTk0LWJjMjMtNTdkZTdlZGUzNzg5XkEyXkFqcGdeQXVyMTkxNjUyNQ@@._V1_.jpg",
            IsActive = true
        }
    };

            _context.Movies.AddRange(movies);
            _context.SaveChanges();

            // Link movies with genres
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

            _context.MovieGenres.AddRange(movieGenres);
            _context.SaveChanges();
        }
    }
}