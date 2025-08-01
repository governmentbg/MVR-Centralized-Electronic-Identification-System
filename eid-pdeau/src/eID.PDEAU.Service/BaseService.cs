using System.Net;
using eID.PDEAU.Contracts.Results;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace eID.PDEAU.Service;

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

    protected static ServiceResult Conflict(string fieldName, object value, string message)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException($"'{nameof(fieldName)}' cannot be null or whitespace.", nameof(fieldName));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
        }

        return new ServiceResult
        {
            StatusCode = HttpStatusCode.Conflict,
            Errors = new List<KeyValuePair<string, string>>
            {
                new (fieldName, $"'{value}' {message}")
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

    protected static ServiceResult Created()
    {
        return new ServiceResult { StatusCode = HttpStatusCode.Created };
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

    protected static ServiceResult<T> ConflictMessage<T>(string fieldName, string message)
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
                new (fieldName, message)
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

    protected static ServiceResult<T> UnhandledException<T>(string error = "Unhandled exception")
    {
        return new ServiceResult<T> { StatusCode = HttpStatusCode.InternalServerError, Error = error };
    }

    protected static ServiceResult<T> Forbidden<T>(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
        }

        return new ServiceResult<T> { StatusCode = HttpStatusCode.Forbidden, Error = message };
    }

    protected static ServiceResult Forbidden(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
        }

        return new ServiceResult { StatusCode = HttpStatusCode.Forbidden, Error = message };
    }

    protected static async Task<IPaginatedData<TEntity>> GetPaginatedAsync<TEntity>(IOrderedQueryable<TEntity> source, int pageIndex, int pageSize)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var totalItems = await source.CountAsync();
        var data = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedData<TEntity>(pageIndex, totalItems, data);
    }

    protected static async Task<IPaginatedData<TEntity>> GetPaginatedAsync<TEntity>(IQueryable<TEntity> source, int pageIndex, int pageSize)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var totalItems = await source.CountAsync();
        var data = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedData<TEntity>(pageIndex, totalItems, data);
    }

    protected static ServiceResult<T> NoContent<T>()
    {
        return new ServiceResult<T> { StatusCode = HttpStatusCode.NoContent };
    }

    private class PaginatedData<T> : IPaginatedData<T>
    {
        public int PageIndex { get; private set; }

        public int TotalItems { get; private set; }

        public IEnumerable<T> Data { get; private set; } = Enumerable.Empty<T>();

        internal PaginatedData(int pageIndex, int totalItems, List<T> data)
        {
            PageIndex = pageIndex;
            TotalItems = totalItems;
            Data = data ?? Enumerable.Empty<T>();
        }
    }
}
