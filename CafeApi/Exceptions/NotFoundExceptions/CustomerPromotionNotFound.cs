namespace CafeApi.Exceptions.NotFoundExceptions;

public class CustomerPromotionNotFound : NotFoundException
{
    public CustomerPromotionNotFound(string? message) : base(message)
    {
    }
}