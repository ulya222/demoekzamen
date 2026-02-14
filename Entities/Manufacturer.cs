using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("manufacturer")]
public partial class Manufacturer
{
    [Key]
    [Column("manufacturerid")]
    public int Manufacturerid { get; set; }

    [Column("manufacturername")]
    [StringLength(50)]
    public string? Manufacturername { get; set; }

    [InverseProperty("ManufacturerNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
