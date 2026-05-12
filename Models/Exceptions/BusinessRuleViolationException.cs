namespace HotelGenericoApi.Models.Exceptions;

public class BusinessRuleViolationException : InvalidOperationException
{
    public BusinessErrorCode ErrorCode { get; }

    public BusinessRuleViolationException(BusinessErrorCode errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessRuleViolationException(BusinessErrorCode errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}