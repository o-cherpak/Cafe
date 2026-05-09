namespace CafeApi.Exceptions.NotFoundExceptions;

public class PromotionNotFound : Exception
{
    public PromotionNotFound(string? message) : base(message)
    {
    }
}