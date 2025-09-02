namespace VentasBros.Application.DTOs.Product
{
    public class CatalogFilterDto
    {
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Sort { get; set; } = "createdAt";
        public string Direction { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

