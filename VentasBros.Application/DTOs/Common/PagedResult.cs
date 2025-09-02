using System;
using System.Collections.Generic;

namespace VentasBros.Application.DTOs.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
    }
}
