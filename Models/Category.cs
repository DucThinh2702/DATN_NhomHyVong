namespace DATN.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
        public string? CategoryImage { get; set; }

        // Navigation property
        public ICollection<Product>? Products { get; set; }
    }
}
