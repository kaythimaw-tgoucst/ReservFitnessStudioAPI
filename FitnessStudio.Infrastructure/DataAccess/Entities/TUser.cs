using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tUser")]
[Index("Email", Name = "UQ_User_Email", IsUnique = true)]
public partial class TUser
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<TBookingWaitlist> TBookingWaitlists { get; set; } = new List<TBookingWaitlist>();

    [InverseProperty("User")]
    public virtual ICollection<TBooking> TBookings { get; set; } = new List<TBooking>();

    [InverseProperty("User")]
    public virtual ICollection<TPackage> TPackages { get; set; } = new List<TPackage>();

    [InverseProperty("User")]
    public virtual ICollection<TUserBusinessStudioMembership> TUserBusinessStudioMemberships { get; set; } = new List<TUserBusinessStudioMembership>();
}
