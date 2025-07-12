namespace Web.Models.DTOs.Request;

public class BookingCreateDTO
{
    public int ShowTimeId { get; set; }
    public List<BookingDetailItemDTO> BookingDetails { get; set; } = new List<BookingDetailItemDTO>();
}

// BookingDetailItemDTO được di chuyển sang file riêng