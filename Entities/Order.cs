using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("ORDER")]
public partial class Order
{
    [Key]
    [Column("orderid")]
    public int Orderid { get; set; }

    [Column("orderstatus")]
    public int? Orderstatus { get; set; }

    [Column("orderaddress")]
    public int? Orderaddress { get; set; }

    [Column("orderdate")]
    public DateOnly? Orderdate { get; set; }

    [Column("orderdateissue")]
    public DateOnly? Orderdateissue { get; set; }

    [Column("orderuser")]
    public int? Orderuser { get; set; }

    [Column("ordercode")]
    public int? Ordercode { get; set; }

    [ForeignKey("Orderaddress")]
    [InverseProperty("Orders")]
    public virtual Address? OrderaddressNavigation { get; set; }

    [InverseProperty("OrderNavigation")]
    public virtual ICollection<Orderproduct> Orderproducts { get; set; } = new List<Orderproduct>();

    [ForeignKey("Orderstatus")]
    [InverseProperty("Orders")]
    public virtual Status? OrderstatusNavigation { get; set; }

    [ForeignKey("Orderuser")]
    [InverseProperty("Orders")]
    public virtual User? OrderuserNavigation { get; set; }
}
