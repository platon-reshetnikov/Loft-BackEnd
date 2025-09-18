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

            var app = builder.Build();

            // Настраиваем конвейер с Ocelot
            app.UseRouting();
            app.UseOcelot().Wait();

            app.Run();
        }
    }
}