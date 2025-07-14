namespace PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions;

public class CustomException(int code,string message) : Exception(message)
{
    public int StatusCode = code;
}