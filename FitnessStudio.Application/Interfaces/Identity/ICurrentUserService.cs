namespace FitnessStudio.Application.Interfaces.Identity
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
    }
}
