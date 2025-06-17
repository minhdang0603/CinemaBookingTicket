using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.Models;

namespace Web.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminMovieController : Controller
    {
        public IActionResult Index()
        {
            var movies = GetSampleMovies();
            ViewBag.Title = "Movie Management";
            return View(movies);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Create New Movie";
            var model = new MovieCreateViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MovieCreateViewModel model)
        {
            TempData["SuccessMessage"] = "Movie created successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Edit Movie";

            var movie = GetSampleMovies().FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                TempData["ErrorMessage"] = "Movie not found!";
                return RedirectToAction("Index");
            }

            var model = new MovieEditViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Rating = movie.Rating,
                PosterUrl = movie.PosterUrl,
                ReleaseDate = movie.ReleaseDate,
                IsNowShowing = movie.IsNowShowing,
                IsFeatured = movie.IsFeatured,
                Price = movie.Price
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MovieEditViewModel model)
        {
            // TODO: Add movie update logic here
            TempData["SuccessMessage"] = "Movie updated successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            ViewBag.Title = "Movie Details";

            // TODO: Get movie by id from database
            var movie = GetSampleMovies().FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                TempData["ErrorMessage"] = "Movie not found!";
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // TODO: Add movie deletion logic here
            TempData["SuccessMessage"] = "Movie deleted successfully!";
            return RedirectToAction("Index");
        }

        #region Private Methods

        private List<Movie> GetSampleMovies()
        {
            return new List<Movie>
            {
                new Movie
                {
                    Id = 1,
                    Title = "Avatar: The Way of Water",
                    Description = "Set more than a decade after the events of the first film, Avatar: The Way of Water begins to tell the story of the Sully family.",
                    Genre = "Action, Adventure, Fantasy",
                    Duration = 192,
                    Rating = 8.5,
                    PosterUrl = "https://image.tmdb.org/t/p/w500/t6HIqrRAclMCA60NsSmeqe9RmNV.jpg",
                    ReleaseDate = new DateTime(2022, 12, 16),
                    IsNowShowing = true,
                    IsFeatured = true,
                    Price = 120000,
                    CreatedAt = DateTime.Now.AddDays(-30),
                    UpdatedAt = DateTime.Now.AddDays(-5)
                },
                new Movie
                {
                    Id = 2,
                    Title = "Top Gun: Maverick",
                    Description = "After thirty years, Maverick is still pushing the envelope as a top naval aviator, but must confront ghosts of his past.",
                    Genre = "Action, Drama",
                    Duration = 130,
                    Rating = 9.2,
                    PosterUrl = "https://image.tmdb.org/t/p/w500/62HCnUTziyWcpDaBO2i1DX17ljH.jpg",
                    ReleaseDate = new DateTime(2022, 5, 27),
                    IsNowShowing = true,
                    IsFeatured = true,
                    Price = 100000,
                    CreatedAt = DateTime.Now.AddDays(-25),
                    UpdatedAt = DateTime.Now.AddDays(-3)
                },
                new Movie
                {
                    Id = 3,
                    Title = "Black Panther: Wakanda Forever",
                    Description = "The people of Wakanda fight to protect their home from intervening world powers as they mourn the death of King T'Challa.",
                    Genre = "Action, Adventure, Drama",
                    Duration = 161,
                    Rating = 8.0,
                    PosterUrl = "https://image.tmdb.org/t/p/w500/sv1xJUazXeYqALzczSZ3O6nkH75.jpg",
                    ReleaseDate = new DateTime(2022, 11, 11),
                    IsNowShowing = true,
                    IsFeatured = false,
                    Price = 110000,
                    CreatedAt = DateTime.Now.AddDays(-20),
                    UpdatedAt = DateTime.Now.AddDays(-2)
                },
                new Movie
                {
                    Id = 4,
                    Title = "Spider-Man: No Way Home",
                    Description = "With Spider-Man's identity now revealed, Peter asks Doctor Strange for help. When a spell goes wrong, dangerous foes from other worlds start to appear.",
                    Genre = "Action, Adventure, Fantasy",
                    Duration = 148,
                    Rating = 9.0,
                    PosterUrl = "https://image.tmdb.org/t/p/w500/1g0dhYtq4irTY1GPXvft6k4YLjm.jpg",
                    ReleaseDate = new DateTime(2021, 12, 17),
                    IsNowShowing = false,
                    IsFeatured = true,
                    Price = 95000,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                },
                new Movie
                {
                    Id = 5,
                    Title = "The Batman",
                    Description = "When a sadistic serial killer begins murdering key political figures in Gotham, Batman is forced to investigate the city's hidden corruption.",
                    Genre = "Action, Crime, Drama",
                    Duration = 176,
                    Rating = 8.8,
                    PosterUrl = "https://image.tmdb.org/t/p/w500/b0PlSFdDwbyK0cf5RxwDpaOJQvQ.jpg",
                    ReleaseDate = new DateTime(2022, 3, 4),
                    IsNowShowing = false,
                    IsFeatured = false,
                    Price = 105000,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    UpdatedAt = DateTime.Now
                },
                new Movie
                {
                    Id = 6,
                    Title = "Doctor Strange in the Multiverse of Madness",
                    Description = "Doctor Strange teams up with a mysterious teenage girl from his dreams who can travel across multiverses.",
                    Genre = "Action, Adventure, Fantasy",
                    Duration = 126,
                    Rating = 7.5,
                    PosterUrl = "https://image.tmdb.org/t/p/w500/9Gtg2DzBhmYamXBS1hKAhiwbBKS.jpg",
                    ReleaseDate = new DateTime(2022, 5, 6),
                    IsNowShowing = true,
                    IsFeatured = false,
                    Price = 90000,
                    CreatedAt = DateTime.Now.AddDays(-8),
                    UpdatedAt = DateTime.Now
                },
                new Movie
                {
                    Id = 7,
                    Title = "Jurassic World Dominion",
                    Description = "Four years after the destruction of Isla Nublar, dinosaurs now live alongside humans around the world.",
                    Genre = "Action, Adventure, Thriller",
                    Duration = 147,
                    Rating = 7.2,
                    PosterUrl = "https://image.tmdb.org/t/p/w500/kAVRgw7GgK1CfYEJq8ME6EvRIgU.jpg",
                    ReleaseDate = new DateTime(2022, 6, 10),
                    IsNowShowing = true,
                    IsFeatured = false,
                    Price = 85000,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now
                },
                new Movie
                {
                    Id = 8,
                    Title = "Minions: The Rise of Gru",
                    Description = "The untold story of one twelve-year-old's dream to become the world's greatest supervillain.",
                    Genre = "Animation, Adventure, Comedy",
                    Duration = 87,
                    Rating = 7.8,
                    PosterUrl = "https://image.tmdb.org/t/p/w500/wKiOkZTN9lUUUNZLmtnwubZYONg.jpg",
                    ReleaseDate = new DateTime(2022, 7, 1),
                    IsNowShowing = true,
                    IsFeatured = true,
                    Price = 80000,
                    CreatedAt = DateTime.Now.AddDays(-3),
                    UpdatedAt = DateTime.Now
                }
            };
        }

        #endregion
    }

    internal class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Duration { get; set; }
        public double Rating { get; set; }
        public string? PosterUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool IsNowShowing { get; set; }
        public bool IsFeatured { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
