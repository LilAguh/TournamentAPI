using Config;
using Microsoft.AspNetCore.Http;
using Models.Exceptions;
using MySqlConnector;
using System.Net;
using System.Text.Json;
using static Models.Exceptions.CustomException;

namespace TournamentApiV2.Middleware
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync (HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var errorCode = "INTERNAL_ERROR";
            var message = ErrorMessages.InternalServerError;

            // Mapeo de excepciones personalizadas
            switch (exception)
            {
                case ValidationException vex:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorCode = "VALIDATION_ERROR";
                    message = vex.Message;
                    break;
                case NotFoundException nex:
                    statusCode = (int)HttpStatusCode.NotFound;
                    errorCode = "NOT_FOUND";
                    message = nex.Message;
                    break;
                case ForbiddenException fex:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    errorCode = "FORBIDDEN";
                    message = fex.Message;
                    break;
                case UnauthorizedException uex:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    errorCode = "UNAUTHORIZED";
                    message = uex.Message;
                    break;
                case InvalidOperationException ioe when ioe.Message.Contains("no existen"):
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorCode = "INVALID_DATA";
                    message = ioe.Message;
                    break;
                case MySqlException myex:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorCode = "DATABASE_ERROR";
                    message = "Error en la base de datos: " + myex.Message;
                    break;
                default:
                    _logger.LogError(exception, "Error no manejado");
                    break;
            }

            // Estructura estándar de respuesta de error
            var response = new
            {
                Code = errorCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
