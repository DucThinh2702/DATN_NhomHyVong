using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = [];

    public virtual User? User { get; set; }
}
