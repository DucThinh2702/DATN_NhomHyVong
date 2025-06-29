using System;
using System.Collections.Generic;

namespace DATN.Models;

public partial class News
{
    public int NewsId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? PostedDate { get; set; }

    public int? AuthorId { get; set; }

    public string? ThumbnailImage { get; set; }

    public virtual User? Author { get; set; }
}
