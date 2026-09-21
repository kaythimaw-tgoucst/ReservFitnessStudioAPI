using FitnessStudio.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FitnessStudio.Infrastructure.DataAccessWriteOnly;

/// <summary>
/// Write-side DbContext used for commands/mutations. Inherits all entity mappings
/// (DbSets, keys, relationships, defaults) from <see cref="FitnessStudioDbContext"/> so both
/// the read and write contexts share the exact same entity classes and model
/// configuration, while remaining distinct types for separate DI registration
/// (e.g. different connection strings, interceptors, or tracking behavior).
/// </summary>
public class FitnessStudioWriteDbContext : FitnessStudioDbContext
{
    public FitnessStudioWriteDbContext(DbContextOptions<FitnessStudioWriteDbContext> options)
        : base(options)
    {
    }
}
