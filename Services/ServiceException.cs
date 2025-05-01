namespace Services
{
    public abstract class ServiceException : Exception
    {
        public ServiceException(string message) : base(message)
        {

        }
    }

    public class ConflictException : ServiceException
    {
        public ConflictException(string message) : base(message)
        {

        }
    }

    public class UnauthorizedException : ServiceException
    {
        public UnauthorizedException(string message) : base(message)
        {

        }
    }

    public class ForbiddenException : ServiceException
    {
        public ForbiddenException(string message) : base(message)
        {

        }
    }

    public class NotFoundException : ServiceException
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }

    public class BadRequestException : ServiceException
    {
        public BadRequestException(string message) : base(message)
        {

        }
    }

}
