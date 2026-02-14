using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("category")]
public partial class Category
{
    [Key]
    [Column("categoryid")]
    public int Categoryid { get; set; }

    [Column("categoryname")]
    [StringLength(50)]
    public string? Categoryname { get; set; }

    [InverseProperty("CategoryNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
