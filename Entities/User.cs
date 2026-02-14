using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("USER")]
public partial class User
{
    [Key]
    [Column("userid")]
    public int Userid { get; set; }

    [Column("role")]
    public int? Role { get; set; }

    [Column("login")]
    [StringLength(50)]
    public string? Login { get; set; }

    [Column("password")]
    [StringLength(50)]
    public string? Password { get; set; }

    [Column("firstname")]
    [StringLength(50)]
    public string? Firstname { get; set; }

    [Column("lastname")]
    [StringLength(50)]
    public string? Lastname { get; set; }

    [Column("middlename")]
    [StringLength(50)]
    public string? Middlename { get; set; }

    [InverseProperty("OrderuserNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("Role")]
    [InverseProperty("Users")]
    public virtual Role? RoleNavigation { get; set; }
}
