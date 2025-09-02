namespace VentasBros.Application.DTOs.Product
{
    public class ProductFilterDto
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Sort { get; set; } = "createdAt";
        public string Direction { get; set; } = "desc";
        public bool OnlyActive { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

