using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DATN.Models;

[Index("Title", Name = "UQ__News__2CB664DC48DCE806", IsUnique = true)]
public partial class News
{
    [Key]
    [Column("NewsID")]
    public int NewsId { get; set; }

    [StringLength(225)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PostedDate { get; set; }

    [Column("AuthorID")]
    public int? AuthorId { get; set; }

    [StringLength(225)]
    public string? ThumbnailImage { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("News")]
    public virtual User? Author { get; set; }
}
