using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tTimetableSchedule")]
[Index("BusinessStudioId", Name = "IX_Schedule_BusinessStudio")]
public partial class TTimetableSchedule
{
    [Key]
    public Guid Id { get; set; }

    public Guid BusinessStudioId { get; set; }

    [StringLength(100)]
    public string ClassName { get; set; } = null!;

    [StringLength(100)]
    public string InstructorName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndTime { get; set; }

    public int Capacity { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedOn { get; set; }

    [ForeignKey("BusinessStudioId")]
    [InverseProperty("TTimetableSchedules")]
    public virtual TBusinessStudio BusinessStudio { get; set; } = null!;

    [InverseProperty("TimetableSchedule")]
    public virtual ICollection<TBookingWaitlist> TBookingWaitlists { get; set; } = new List<TBookingWaitlist>();

    [InverseProperty("TimetableSchedule")]
    public virtual ICollection<TBooking> TBookings { get; set; } = new List<TBooking>();
}
