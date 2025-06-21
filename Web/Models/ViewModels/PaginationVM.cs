using System;

namespace Web.Models.ViewModels;

public class PaginationVM
{
    public int currentPage { get; set; }

    public int countPages { get; set; }

    public Func<int?, string> generateUrl { get; set; }
}
