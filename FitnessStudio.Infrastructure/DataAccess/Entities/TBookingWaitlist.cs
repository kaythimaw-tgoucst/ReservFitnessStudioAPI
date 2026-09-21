using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tBookingWaitlist")]
[Index("TimetableScheduleId", "JoinedAt", Name = "IX_Waitlist_Schedule_JoinedAt")]
public partial class TBookingWaitlist
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid TimetableScheduleId { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime JoinedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PromotedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    [ForeignKey("TimetableScheduleId")]
    [InverseProperty("TBookingWaitlists")]
    public virtual TTimetableSchedule TimetableSchedule { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TBookingWaitlists")]
    public virtual TUser User { get; set; } = null!;
}
