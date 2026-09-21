using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessStudio.Infrastructure.DataAccess.Entities;

[Table("tUserBusinessStudioMembership")]
public partial class TUserBusinessStudioMembership
{
    public Guid UserId { get; set; }

    public Guid BusinessStudioId { get; set; }

    public string? Role { get; set; }

    public bool IsActive { get; set; } = true;

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual TUser User { get; set; } = null!;

    [ForeignKey(nameof(BusinessStudioId))]
    public virtual TBusinessStudio BusinessStudio { get; set; } = null!;
}
