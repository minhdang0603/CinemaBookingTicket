using System;

namespace API.DTOs.Response;

public class ConcessionOrderDetailDTO
{
    public int Id { get; set; }
    public int ConcessionId { get; set; }
    public string ConcessionName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}