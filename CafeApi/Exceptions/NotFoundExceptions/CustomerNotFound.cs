namespace CafeApi.Exceptions.NotFoundExceptions;

public class CustomerNotFound : NotFoundException
{
    public CustomerNotFound(string? message) : base(message)
    {
    }
}