using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Entities;

[Table("role")]
public partial class Role
{
    [Key]
    [Column("roleid")]
    public int Roleid { get; set; }

    [Column("rolename")]
    [StringLength(50)]
    public string? Rolename { get; set; }

    [InverseProperty("RoleNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
