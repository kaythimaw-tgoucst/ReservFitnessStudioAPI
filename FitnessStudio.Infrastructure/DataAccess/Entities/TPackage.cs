using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tPackage")]
[Index("BusinessStudioId", Name = "IX_Package_BusinessStudio")]
[Index("UserId", Name = "IX_Package_User")]
public partial class TPackage
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BusinessStudioId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalCredits { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal RemainingCredits { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExpiryDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    [ForeignKey("BusinessStudioId")]
    [InverseProperty("TPackages")]
    public virtual TBusinessStudio BusinessStudio { get; set; } = null!;

    [InverseProperty("Package")]
    public virtual ICollection<TBooking> TBookings { get; set; } = new List<TBooking>();

    [ForeignKey("UserId")]
    [InverseProperty("TPackages")]
    public virtual TUser User { get; set; } = null!;
}
