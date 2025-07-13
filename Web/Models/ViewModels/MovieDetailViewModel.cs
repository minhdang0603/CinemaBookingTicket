using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
    public class ShowtimesByScreenGroup
    {
        public required ScreenDTO Screen { get; set; }
        public required List<ShowTimeLiteDTO> Showtimes { get; set; }
    }

    public class ShowtimesByTheaterGroup
    {
        public required TheaterDTO Theater { get; set; }
        public required List<ShowtimesByScreenGroup> Screens { get; set; }
    }

    public class MovieDetailViewModel
    {
        public MovieDTO Movie { get; set; } = new MovieDTO();
        public List<ShowTimeLiteDTO> ShowTimes { get; set; } = new List<ShowTimeLiteDTO>();
        public List<ShowTimeLiteDTO> AllShowTimes { get; set; } = new List<ShowTimeLiteDTO>(); // For filter options
        public List<ProvinceDTO> AllProvinces { get; set; } = new List<ProvinceDTO>(); // All provinces from database
        public DateOnly? SelectedDate { get; set; }
        public int? SelectedProvinceId { get; set; }
        public bool HasShowtimes => ShowTimes?.Any() == true;

        /// <summary>
        /// Gets all provinces from database for filter dropdown
        /// </summary>
        public IEnumerable<ProvinceDTO> AvailableProvinces
        {
            get
            {
                if (AllProvinces == null)
                    return Enumerable.Empty<ProvinceDTO>();

                return AllProvinces.OrderBy(p => p.Name);
            }
        }

        /// <summary>
        /// Gets available dates from all showtimes for date filter (next 5 days only)
        /// </summary>
        public IEnumerable<DateOnly> AvailableDates
        {
            get
            {
                if (AllShowTimes == null)
                    return Enumerable.Empty<DateOnly>();

                var today = DateOnly.FromDateTime(DateTime.Today);
                var maxDate = today.AddDays(4); // 5 ngày kể từ hôm nay (0-4 days)

                return AllShowTimes
                    .Select(st => st.ShowDate)
                    .Where(date => date >= today && date <= maxDate)
                    .Distinct()
                    .OrderBy(d => d);
            }
        }

        /// <summary>
        /// Groups showtimes by theater, then by screen
        /// </summary>
        public IEnumerable<ShowtimesByTheaterGroup> ShowtimesByTheater
        {
            get
            {
                if (ShowTimes == null)
                    return Enumerable.Empty<ShowtimesByTheaterGroup>();

                return ShowTimes
                    .Where(st => st.Screen != null && st.Screen.Theater != null)
                    .GroupBy(st => st.Screen!.Theater!.Id)
                    .Select(theaterGroup => new ShowtimesByTheaterGroup
                    {
                        Theater = theaterGroup.First().Screen!.Theater!,
                        Screens = theaterGroup
                            .GroupBy(st => st.Screen!.Id)
                            .Select(screenGroup => new ShowtimesByScreenGroup
                            {
                                Screen = screenGroup.First().Screen!,
                                Showtimes = screenGroup.OrderBy(st => st.ShowDate).ThenBy(st => st.StartTime).ToList()
                            })
                            .OrderBy(sg => sg.Screen.Name)
                            .ToList()
                    })
                    .OrderBy(g => g.Theater.Name);
            }
        }

        /// <summary>
        /// Check if a showtime should be disabled (past time for today)
        /// </summary>
        /// <param name="showtime">The showtime to check</param>
        /// <returns>True if the showtime should be disabled</returns>
        public bool IsShowtimeDisabled(ShowTimeLiteDTO showtime)
        {
            if (showtime.ShowDate != DateOnly.FromDateTime(DateTime.Today))
                return false; // Only disable for today

            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
            return showtime.StartTime <= currentTime;
        }
    }
}
