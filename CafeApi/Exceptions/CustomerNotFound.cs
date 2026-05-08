namespace CafeApi.Exceptions;

public class CustomerNotFound : Exception
{
    public CustomerNotFound(string? message) : base(message)
    {
    }
}