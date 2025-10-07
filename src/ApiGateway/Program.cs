using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавляем Ocelot для маршрутизации
            builder.Configuration.AddJsonFile("ocelot.json");
            builder.Services.AddOcelot();

            // Добавляем CORS политику для фронтенда
            var allowedOrigins = new[] { "https://your.frontend.domain", "http://localhost:3000" };
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Настраиваем конвейер с Ocelot
            app.UseRouting();

            // Включаем CORS перед Ocelot
            app.UseCors("FrontendPolicy");

            app.UseOcelot().Wait();

            app.Run();
        }
    }
}