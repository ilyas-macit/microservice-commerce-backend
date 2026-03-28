namespace ApiGateway.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            if (_environment.IsDevelopment())
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
            else
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "An error occurred"
                });
            }
        }
    }
}
