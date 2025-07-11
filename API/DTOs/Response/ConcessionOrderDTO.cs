using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Response;

public class ConcessionOrderDTO
{
    public int Id { get; set; }
    public int? BookingId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public List<ConcessionOrderDetailDTO> ConcessionOrderItems { get; set; } = new List<ConcessionOrderDetailDTO>();
}
