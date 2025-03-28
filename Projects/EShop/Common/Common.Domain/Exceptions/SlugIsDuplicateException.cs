namespace Common.Domain.Exceptions
{
    public class SlugIsDuplicateException : BaseDomainException
    {
        public SlugIsDuplicateException() : base("Duplicate slug!")
        {
        }

        public SlugIsDuplicateException(string message) : base(message)
        {
        }
    }
}