namespace DATN.Models
{
    public class Color
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }

        // Navigation
        public ICollection<ProductVariant>? ProductVariants { get; set; }
    }

}
