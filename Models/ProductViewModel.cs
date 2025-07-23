namespace DATN.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ThumbnailImage { get; set; }
        public decimal? SalePrice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Stock { get; set; }
    }
}
