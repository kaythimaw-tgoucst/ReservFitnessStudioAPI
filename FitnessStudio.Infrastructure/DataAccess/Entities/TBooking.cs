using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tBooking")]
[Index("TimetableScheduleId", Name = "IX_Booking_Schedule")]
[Index("UserId", Name = "IX_Booking_User")]
[Index("BookingNo", Name = "UQ_Booking_BookingNo", IsUnique = true)]
public partial class TBooking
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(30)]
    public string BookingNo { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid TimetableScheduleId { get; set; }

    public Guid PackageId { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime BookedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CancelledOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    [ForeignKey("PackageId")]
    [InverseProperty("TBookings")]
    public virtual TPackage Package { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<TTransaction> TTransactions { get; set; } = new List<TTransaction>();

    [ForeignKey("TimetableScheduleId")]
    [InverseProperty("TBookings")]
    public virtual TTimetableSchedule TimetableSchedule { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TBookings")]
    public virtual TUser User { get; set; } = null!;
}
