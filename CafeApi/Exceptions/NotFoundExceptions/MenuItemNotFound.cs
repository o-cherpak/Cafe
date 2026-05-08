namespace CafeApi.Exceptions.NotFoundExceptions;

public class MenuItemNotFound : NotFoundException
{
    public MenuItemNotFound(string? message) : base(message)
    {
    }
}