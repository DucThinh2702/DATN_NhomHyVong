using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? OrderId { get; set; }

    public int? PaymentMethodId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? BankTransactionCode { get; set; }

    public string? PaymentContent { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentType { get; set; }

    public virtual Order? Order { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }
}
