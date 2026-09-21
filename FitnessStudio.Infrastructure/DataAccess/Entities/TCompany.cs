using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tCompany")]
public partial class TCompany
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    [InverseProperty("Company")]
    public virtual ICollection<TBusinessStudio> TBusinessStudios { get; set; } = new List<TBusinessStudio>();

}
