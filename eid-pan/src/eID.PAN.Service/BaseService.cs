using System.Net;
using eID.PAN.Contracts.Results;
using FluentValidation.Results;

namespace eID.PAN.Service;

public class BaseService
{
    protected static ServiceResult Accepted()
    {
        return new ServiceResult { StatusCode = HttpStatusCode.Accepted };
    }
    protected static ServiceResult Ok()
    {
        return new ServiceResult { StatusCode = HttpStatusCode.OK };
    }

    protected static ServiceResult NoContent()
    {
        return new ServiceResult { StatusCode = HttpStatusCode.NoContent };
    }

    protected static ServiceResult NotModified()
    {
        return new ServiceResult { StatusCode = HttpStatusCode.NotModified };
    }

    protected static ServiceResult BadRequest(List<ValidationFailure> errors)
    {
        if (errors is null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        return new ServiceResult
        {
            StatusCode = HttpStatusCode.BadRequest,
            Errors = new List<KeyValuePair<string, string>>(
                errors.Select(e =>
                    new KeyValuePair<string, string>(e.PropertyName, e.ErrorMessage)))
        };
    }

    protected static ServiceResult BadRequest(string fieldName, string problem)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        if (string.IsNullOrWhiteSpace(problem))
        {
            throw new ArgumentException($"'{nameof(problem)}' cannot be null or whitespace.", nameof(problem));
        }

        return new ServiceResult
        {
            StatusCode = HttpStatusCode.BadRequest,
            Errors = new List<KeyValuePair<string, string>>
            {
                new (fieldName, problem)
            }
        };
    }

    protected static ServiceResult Conflict(string fieldName, object value)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        return new ServiceResult
        {
            StatusCode = HttpStatusCode.Conflict,
            Errors = new List<KeyValuePair<string, string>>
            {
                new (fieldName, $"'{value}' already exist")
            }
        };
    }

    protected static ServiceResult NotFound(string fieldName, object value)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        return new ServiceResult
        {
            StatusCode = HttpStatusCode.NotFound,
            Errors = new List<KeyValuePair<string, string>>
            {
                new (fieldName, $"'{value}' not found")
            }
        };
    }
    protected static ServiceResult InternalServerError(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
        }

        return new ServiceResult { StatusCode = HttpStatusCode.InternalServerError, Error = message };
    }

    protected static ServiceResult UnhandledException()
    {
        return new ServiceResult { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" };
    }

    protected static ServiceResult<T> Accepted<T>(T result)
    {
        return new ServiceResult<T> { Result = result, StatusCode = HttpStatusCode.Accepted };
    }

    protected static ServiceResult<T> Ok<T>(T result)
    {
        return new ServiceResult<T> { Result = result, StatusCode = HttpStatusCode.OK };
    }

    protected static ServiceResult<T> Created<T>(T result)
    {
        return new ServiceResult<T> { Result = result, StatusCode = HttpStatusCode.Created };
    }

    protected static ServiceResult<T> NotModified<T>(T result)
    {
        return new ServiceResult<T> { Result = result, StatusCode = HttpStatusCode.NotModified };
    }

    protected static ServiceResult<T> BadRequest<T>(List<ValidationFailure> errors)
    {
        if (errors is null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        return new ServiceResult<T>
        {
            StatusCode = HttpStatusCode.BadRequest,
            Errors = new List<KeyValuePair<string, string>>(
                errors.Select(e =>
                    new KeyValuePair<string, string>(e.PropertyName, e.ErrorMessage)))
        };
    }
    /// <summary>
    /// Use it for "already exists" type of conflicts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    protected static ServiceResult<T> Conflict<T>(string fieldName, object value)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        return new ServiceResult<T>
        {
            StatusCode = HttpStatusCode.Conflict,
            Errors = new List<KeyValuePair<string, string>>
            {
                new (fieldName, $"'{value}' already exist")
            }
        };
    }

    protected static ServiceResult<T> Conflict<T>(string fieldName, object value, string message)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
        }

        return new ServiceResult<T>
        {
            StatusCode = HttpStatusCode.Conflict,
            Errors = new List<KeyValuePair<string, string>>
            {
                new (fieldName, $"'{value}' {message}")
            }
        };
    }

    protected static ServiceResult<T> NotFound<T>(string fieldName, object value)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        return new ServiceResult<T>
        {
            StatusCode = HttpStatusCode.NotFound,
            Errors = new List<KeyValuePair<string, string>>
            {
                new (fieldName, $"'{value}' not found")
            }
        };
    }

    protected static ServiceResult<T> UnhandledException<T>()
    {
        return new ServiceResult<T> { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" };
    }
}
