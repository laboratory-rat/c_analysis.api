using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MRCryptoCurrencyAnalysis.Init
{
    public static class CorsProfile
    {
        public static void ConfigCors(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddCors(c =>
            {
                c.AddPolicy("ALL", pb => pb
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());

            });
        }

    }
}
