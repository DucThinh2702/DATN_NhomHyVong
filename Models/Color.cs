namespace DATN.Models
{
    public class Color
    {
        public int ColorID { get; set; }
        public string ColorName { get; set; }

        // Navigation
        public ICollection<ProductVariant>? ProductVariants { get; set; }
    }

}
