using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("status")]
public partial class Status
{
    [Key]
    [Column("statusid")]
    public int Statusid { get; set; }

    [Column("statusname")]
    [StringLength(50)]
    public string? Statusname { get; set; }

    [InverseProperty("OrderstatusNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
