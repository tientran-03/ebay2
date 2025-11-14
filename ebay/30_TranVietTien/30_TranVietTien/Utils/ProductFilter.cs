namespace _30_TranVietTien.Services
{
    public class ProductFilter
    {
        public int? CategoryId { get; set; }
        public string? Q { get; set; } 
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public string? Sort { get; set; } = "newest";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
