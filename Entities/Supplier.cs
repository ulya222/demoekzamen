using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("supplier")]
public partial class Supplier
{
    [Key]
    [Column("supplierid")]
    public int Supplierid { get; set; }

    [Column("suppliername")]
    [StringLength(50)]
    public string? Suppliername { get; set; }

    [InverseProperty("SupplierNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
