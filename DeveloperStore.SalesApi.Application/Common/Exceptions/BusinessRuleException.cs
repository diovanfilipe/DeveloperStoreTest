namespace DeveloperStore.SalesApi.Application.Common.Exceptions;

public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}
