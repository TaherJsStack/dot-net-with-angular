using Microsoft.Extensions.Localization;
using System.Globalization;

namespace dotnetAPI.Middleware
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStringLocalizer<LocalizationMiddleware> _localizer;

        public LocalizationMiddleware(RequestDelegate next, IStringLocalizer<LocalizationMiddleware> localizer)
        {
            _next = next;
            _localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cultureQuery = context.Request.Headers["Accept-Language"].ToString();
            Console.WriteLine("cultureQuery ===> ", cultureQuery);
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                var culture = new CultureInfo(cultureQuery);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }

            await _next(context);
        }
    }
}
