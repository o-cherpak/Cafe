namespace CafeApi.Exceptions;

public class PromotionNotActiveException : Exception
{
    public PromotionNotActiveException(string message) : base(message)
    {
    }
}