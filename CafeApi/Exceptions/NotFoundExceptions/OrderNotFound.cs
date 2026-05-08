namespace CafeApi.Exceptions.NotFoundExceptions;

public class OrderNotFound : NotFoundException
{
    public OrderNotFound(string? message) : base(message)
    {
    }
}