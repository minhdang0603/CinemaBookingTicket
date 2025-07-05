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
        public bool HasShowtimes => ShowTimes?.Any() == true;

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
