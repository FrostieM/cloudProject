using System;

namespace cloudSite.ViewData
{
    public class PaginationViewData
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int Pages => (int)Math.Ceiling((decimal)TotalItems / PageSize);
    }
}