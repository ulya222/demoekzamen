using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("orderproduct")]
public partial class Orderproduct
{
    [Key]
    [Column("orderproductid")]
    public int Orderproductid { get; set; }

    [Column("ORDER")]
    public int? Order { get; set; }

    [Column("product")]
    [StringLength(10)]
    public string? Product { get; set; }

    [Column("orderproductcount")]
    public int? Orderproductcount { get; set; }

    [ForeignKey("Order")]
    [InverseProperty("Orderproducts")]
    public virtual Order? OrderNavigation { get; set; }

    [ForeignKey("Product")]
    [InverseProperty("Orderproducts")]
    public virtual Product? ProductNavigation { get; set; }
}
