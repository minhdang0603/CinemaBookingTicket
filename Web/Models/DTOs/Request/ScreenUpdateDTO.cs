using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.DTOs.Request
{
    public class ScreenUpdateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phòng chiếu là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên phòng chiếu không được vượt quá 50 ký tự")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Số hàng là bắt buộc")]
        [Range(1, 26, ErrorMessage = "Số hàng phải từ 1 đến 26")]
        public int? Rows { get; set; }

        [Required(ErrorMessage = "Số ghế mỗi hàng là bắt buộc")]
        [Range(1, 20, ErrorMessage = "Số ghế mỗi hàng phải từ 1 đến 20")]
        public int? SeatsPerRow { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn rạp chiếu")]
        public int? TheaterId { get; set; }

        public List<SeatUpdateDTO>? Seats { get; set; } = new List<SeatUpdateDTO>();
    }
}
