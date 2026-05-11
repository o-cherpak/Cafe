namespace CafeApi.Exceptions;

public class InsufficientBonusException : Exception
{
    public InsufficientBonusException(string? message) : base(message)
    {
    }
}