namespace FitnessStudio.Application.Exceptions
{
    public class UserNotInBusinessStudioException : Exception
    {
        public UserNotInBusinessStudioException(Guid userId, Guid businessStudioId)
            : base($"The user is not a member of the selected business.")
        {
        }
    }
}
