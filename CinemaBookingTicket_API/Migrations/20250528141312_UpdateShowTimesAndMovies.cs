using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBookingTicket_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShowTimesAndMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "UK_ShowTimes_Movie_Screen_Date_Time",
                table: "ShowTimes",
                columns: new[] { "MovieId", "ScreenId", "ShowDate", "StartTime" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
