using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("address")]
public partial class Address
{
    [Key]
    [Column("addressid")]
    public int Addressid { get; set; }

    [Column("addressname")]
    [StringLength(200)]
    public string? Addressname { get; set; }

    [InverseProperty("OrderaddressNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
