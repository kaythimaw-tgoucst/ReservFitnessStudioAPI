namespace FitnessStudio.Application.Interfaces.Repositories.ReadOnly
{
    public interface IUserReadOnlyRepository
    {
        Task<Guid?> GetUserIdByEmailAsync(string email);
    }
}
