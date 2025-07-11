using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request;

public class ConcessionOrderCreateDTO
{
    [Required]
    public int? BookingId { get; set; }

    [Required]
    public List<ConcessionOrderDetailItemDTO> ConcessionOrderDetails { get; set; } = new List<ConcessionOrderDetailItemDTO>();
}

public class ConcessionOrderDetailItemDTO
{
    [Required]
    public int ConcessionId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "UnitPrice must be greater than 0")]
    public decimal UnitPrice { get; set; }
}
