using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels
{
    public class MovieDetailViewModel
    {
        public MovieDetailDTO Movie { get; set; } = new MovieDetailDTO();
        public bool HasShowtimes => Movie?.ShowTimes?.Any() == true;

        /// <summary>
        /// Groups showtimes by theater for display purposes
        /// </summary>
        public IEnumerable<IGrouping<TheaterDTO, ShowTimeLiteDTO>> ShowtimesByTheater
        {
            get
            {
                if (Movie?.ShowTimes == null)
                    return Enumerable.Empty<IGrouping<TheaterDTO, ShowTimeLiteDTO>>();

                return Movie.ShowTimes
                    .Where(st => st.Screen?.Theater != null)
                    .GroupBy(st => st.Screen!.Theater!)
                    .OrderBy(group => group.Key.Name);
            }
        }
    }
}
