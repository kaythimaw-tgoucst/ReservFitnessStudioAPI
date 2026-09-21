namespace FitnessStudio.Application.Exceptions
{
    public class BookingException : Exception
    {
        public string ErrorCode { get; }

        public BookingException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
