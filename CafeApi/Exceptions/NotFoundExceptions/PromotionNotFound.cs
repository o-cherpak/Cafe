namespace CafeApi.Exceptions.NotFoundExceptions;

public class PromotionNotFound : NotFoundException
{
    public PromotionNotFound(string? message) : base(message)
    {
    }
}