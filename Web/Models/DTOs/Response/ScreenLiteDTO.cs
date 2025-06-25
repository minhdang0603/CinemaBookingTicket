namespace Web.Models.DTOs.Response
{
    /// <summary>
    /// A lightweight version of ScreenDTO without Theater information to prevent circular references
    /// </summary>
    public class ScreenLiteDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
    }
}
