using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tBusinessStudio")]
public partial class TBusinessStudio
{
    [Key]
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    [ForeignKey("CompanyId")]
    [InverseProperty("TBusinessStudios")]
    public virtual TCompany Company { get; set; } = null!;

    [InverseProperty("BusinessStudio")]
    public virtual ICollection<TNumberingFormat> TNumberingFormats { get; set; } = new List<TNumberingFormat>();

    [InverseProperty("BusinessStudio")]
    public virtual ICollection<TPackage> TPackages { get; set; } = new List<TPackage>();

    [InverseProperty("BusinessStudio")]
    public virtual ICollection<TUserBusinessStudioMembership> TUserBusinessStudioMemberships { get; set; } = new List<TUserBusinessStudioMembership>();

    [InverseProperty("BusinessStudio")]
    public virtual ICollection<TTimetableSchedule> TTimetableSchedules { get; set; } = new List<TTimetableSchedule>();

    [InverseProperty("BusinessStudio")]
    public virtual ICollection<TTransaction> TTransactions { get; set; } = new List<TTransaction>();
}
