using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tNumberingFormat")]
[Index("BusinessStudioId", "VoucherType", "YearMonth", Name = "UQ_NumberingFormat_Scope", IsUnique = true)]
public partial class TNumberingFormat
{
    [Key]
    public Guid Id { get; set; }

    public Guid BusinessStudioId { get; set; }

    [StringLength(20)]
    public string VoucherType { get; set; } = null!;

    [StringLength(10)]
    public string Prefix { get; set; } = null!;

    [StringLength(50)]
    public string? FormatPattern { get; set; }

    [StringLength(6)]
    [Unicode(false)]
    public string YearMonth { get; set; } = null!;

    public int LastNumber { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    [ForeignKey("BusinessStudioId")]
    [InverseProperty("TNumberingFormats")]
    public virtual TBusinessStudio BusinessStudio { get; set; } = null!;
}
