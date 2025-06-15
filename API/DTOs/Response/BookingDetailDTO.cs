namespace API.DTOs.Response
{
    public class BookingDetailDTO
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingStatus { get; set; }
        public decimal TotalAmount { get; set; }

        public MovieDTO Movie { get; set; }

        public DateTime ShowDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public decimal BasePrice { get; set; }

        public List<BookedSeatDTO> BookedSeats { get; set; }

        public PaymentDTO Payment { get; set; }


        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }

    public class BookedSeatDTO
    {
        public string SeatName { get; set; }
        public string SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public string SeatTypeName { get; set; }
        public decimal SeatPrice { get; set; }
    }

    
    public class ConcessionOrderDTO
    {
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public List<ConcessionItemDTO> Items { get; set; }
    }

    public class ConcessionItemDTO
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
