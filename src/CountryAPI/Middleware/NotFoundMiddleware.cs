using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CountryAPI
{
    public class NotFoundMiddleware
    {
        private readonly RequestDelegate _next;

        public NotFoundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    error = "Resource not found",
                    message = "The requested URL was not found on the server.",
                    availableEndpoints = new[]
                    {
                    new { path = "/api/countries", description = "Get a list of all countries." },
                    new { path = "/api/countries/{code}", description = "Get details of a country by code." },
                    new { path = "/api/regions", description = "Get a list of regions and their countries." },
                    new { path = "/api/languages", description = "Get a list of languages and the countries where they are spoken." }
                },
                    documentation = "/swagger"
                };

                // Reset the status code to 404 and write the response
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}

