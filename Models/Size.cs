namespace DATN.Models
{
    public class Size
    {
        public int SizeID { get; set; }
        public string SizeName { get; set; }

        // Navigation
        public ICollection<ProductVariant>? ProductVariants { get; set; }
    }

}
