using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace dotnetAPI.Middleware
{
    public class HeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Inspect and modify incoming request headers
            var requestHeaders = context.Request.Headers;
            // Example: Log incoming headers
            foreach (var header in requestHeaders)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }

            // Add or modify headers in the request if needed
            // Example: Add a custom header to the request
            //context.Request.Headers["X-Custom-Request-Header"] = "CustomValue";

            // Add custom headers to the response before the response starts
            context.Response.OnStarting(() =>
            {
                var responseHeaders = context.Response.Headers;

                // Add CORS headers
                responseHeaders["Access-Control-Allow-Origin"] = "*";
                responseHeaders["Access-Control-Allow-Headers"] = "Origin, X-Requested-With, Content-Type, Accept, Authorization, AppLanguage, Allowencrypt, http://localhost:4200";
                responseHeaders["Access-Control-Allow-Methods"] = "GET, POST, PATCH, PUT, DELETE, OPTIONS";

                // Set the Referrer-Policy header
                //responseHeaders["Referrer-Policy"] = "strict-origin-when-cross-origin";

                // Example: Add a custom header to the response
                //responseHeaders["X-Custom-Response-Header"] = "CustomValue";

                return Task.CompletedTask;
            });

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
