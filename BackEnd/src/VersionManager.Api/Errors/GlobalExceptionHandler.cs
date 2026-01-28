using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VersionManager.Domain.Common;

namespace VersionManager.Api.Errors;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            DbUpdateException dbEx when IsUniqueConstraintViolation(dbEx) => (StatusCodes.Status409Conflict, "Conflito de dados"),
            _ => (StatusCodes.Status500InternalServerError, "Erro inesperado")
        };

        var detail = status == StatusCodes.Status500InternalServerError
            ? "Ocorreu um erro inesperado."
            : exception.Message;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{status}",
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sql)
            return sql.Number is 2601 or 2627;

        return false;
    }
}
