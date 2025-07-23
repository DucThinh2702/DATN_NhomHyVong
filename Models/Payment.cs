using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

[Index("BankTransactionCode", Name = "UQ__Payments__203D5DBA8A19F839", IsUnique = true)]
public partial class Payment
{
    [Key]
    [Column("PaymentID")]
    public int PaymentId { get; set; }

    [Column("OrderID")]
    public int? OrderId { get; set; }

    [Column("PaymentMethodID")]
    public int? PaymentMethodId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [StringLength(50)]
    public string? BankTransactionCode { get; set; }

    [StringLength(225)]
    public string? PaymentContent { get; set; }

    [StringLength(50)]
    public string? PaymentStatus { get; set; }

    [StringLength(50)]
    public string? PaymentType { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order? Order { get; set; }

    [ForeignKey("PaymentMethodId")]
    [InverseProperty("Payments")]
    public virtual PaymentMethod? PaymentMethod { get; set; }
}
