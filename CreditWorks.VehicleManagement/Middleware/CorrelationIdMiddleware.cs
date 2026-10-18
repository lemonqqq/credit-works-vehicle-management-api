namespace CreditWorks.VehicleManagement.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-ID";
    private const int MaxLength = 128;

    public async Task InvokeAsync(HttpContext context)
    {
        var suppliedId = context.Request.Headers[HeaderName];
        var correlationId = suppliedId.Count == 1 && IsValid(suppliedId[0])
            ? suppliedId[0]!
            : Guid.NewGuid().ToString("N");

        context.Response.Headers[HeaderName] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = correlationId
               }))
        {
            try
            {
                await next(context);
                logger.LogInformation(
                    "HTTP {Method} {Path} completed with {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "HTTP {Method} {Path} failed",
                    context.Request.Method,
                    context.Request.Path);
                throw;
            }
        }
    }

    private static bool IsValid(string? value) =>
        !string.IsNullOrEmpty(value) &&
        value.Length <= MaxLength &&
        value.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_');
}
