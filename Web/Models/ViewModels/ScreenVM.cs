using System;
using Web.Models.DTOs.Response;

namespace Web.Models.ViewModels;

public class ScreenVM
{
    public List<ScreenDTO> ScreenList { get; set; }
    public PaginationVM Pagination { get; set; } = new PaginationVM();
}
