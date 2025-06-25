namespace API.DTOs.Request
{
    public class SeatUpdateDTO
    {
        public int Id { get; set; }
        public string SeatRow { get; set; } = null!;
        public int SeatNumber { get; set; }
        public int SeatTypeId { get; set; }
    }
}
