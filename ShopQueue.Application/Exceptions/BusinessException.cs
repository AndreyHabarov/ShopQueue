namespace ShopQueue.Application.Exceptions;

public class BusinessException(string message) : Exception(message)
{
}