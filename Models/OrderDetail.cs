using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; } 

        [Required]
        public int OrderID { get; set; }      

        [Required]
        public int ProductID { get; set; }     

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
    }
}
