public class APIKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string APIKeyHeaderName = "X-API-KEY";
    private const string APIKey = "Khalil-GestionCommande-2025";

    public APIKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(APIKeyHeaderName, out var extractedApiKey) || extractedApiKey != APIKey)
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized client");
            return;
        }

        await _next(context);
    }
}