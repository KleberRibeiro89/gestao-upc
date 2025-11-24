using FluentValidation.Results;


namespace GestaoUpc.Domain.DTOs.Responses;

public record ResponseBase
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ICollection<ErrorValidation>? Errors { get; set; }

    public static ResponseBase RequestError(string message, ValidationResult validationResult)
    {
        return new ResponseBase
        {
            Success = false,
            Message = message,
            Errors = [.. validationResult.Errors.Select(x => new ErrorValidation { Field = x.PropertyName, Message = x.ErrorMessage })],
        };
    }

    public static ResponseBase<T> RequestError<T>(string message, ValidationResult validationResult)
    {
        return new ResponseBase<T>
        {
            Success = false,
            Message = message,
            Errors = validationResult.Errors.Select(x => new ErrorValidation { Field = x.PropertyName, Message = x.ErrorMessage }).ToList(),
        };
    }

    public static ResponseBase Fail(string message) => new() { Success = false, Message = message };
    public static ResponseBase<T> Fail<T>(string message) => new() { Success = false, Message = message };

    public static ResponseBase Ok() => new() { Success = true, Message = "OK" };
    public static ResponseBase Ok(string message) => new() { Success = true, Message = message };
    public static ResponseBase<T> Ok<T>(T data) => new() { Success = true, Data = data };
    public static ResponseBase<T> Ok<T>(string message, T data) => new() { Success = true, Message = message, Data = data };

}

public record ResponseBase<T> : ResponseBase
{
    public T? Data { get; set; }
}
