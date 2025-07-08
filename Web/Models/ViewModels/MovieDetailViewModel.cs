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
        public DateOnly? SelectedDate { get; set; }
        public int? SelectedProvinceId { get; set; }
        public bool HasShowtimes => ShowTimes?.Any() == true;

        /// <summary>
        /// Gets provinces from all showtimes (theaters' provinces)
        /// </summary>
        public IEnumerable<ProvinceDTO> AvailableProvinces
        {
            get
            {
                if (AllShowTimes == null)
                    return Enumerable.Empty<ProvinceDTO>();

                return AllShowTimes
                    .Where(st => st.Screen?.Theater?.Province != null)
                    .Select(st => st.Screen!.Theater!.Province!)
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .OrderBy(p => p.Name);
            }
        }

        /// <summary>
        /// Gets available dates from all showtimes for date filter
        /// </summary>
        public IEnumerable<DateOnly> AvailableDates
        {
            get
            {
                if (AllShowTimes == null)
                    return Enumerable.Empty<DateOnly>();

                return AllShowTimes
                    .Select(st => st.ShowDate)
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
    }
}
