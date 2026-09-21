using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tTransaction")]
[Index("TransactionNo", Name = "UQ_Transaction_TransactionNo", IsUnique = true)]
public partial class TTransaction
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(30)]
    public string TransactionNo { get; set; } = null!;

    public Guid BookingId { get; set; }

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

    [ForeignKey("BookingId")]
    [InverseProperty("TTransactions")]
    public virtual TBooking Booking { get; set; } = null!;

    [ForeignKey("BusinessStudioId")]
    [InverseProperty("TTransactions")]
    public virtual TBusinessStudio BusinessStudio { get; set; } = null!;
}
