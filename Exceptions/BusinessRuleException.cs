namespace CarPartsProject.Exceptions;

public class BusinessRuleException(string message, int StatusCode) : Exception(message)
{
    public int StatusCode { get; } = StatusCode;
}