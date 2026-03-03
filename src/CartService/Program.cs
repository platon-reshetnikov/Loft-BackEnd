using CartService.Data;
using CartService.Mappings;
using CartService.Services;
using Microsoft.EntityFrameworkCore;

namespace CartService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(CartProfile));

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CartService API", Version = "v1" });
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var cad = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (cad == null) return false;
                    return cad.ControllerTypeInfo.Assembly == typeof(Program).Assembly;
                });
                c.MapType<Microsoft.AspNetCore.Http.IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Format = "binary" });
            });
            
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<CartDbContext>(options =>
                options.UseNpgsql(connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    }));
            
            builder.Services.AddScoped<ICartService, CartService.Services.CartService>();
            
            builder.Services.AddTransient<ServiceAuthenticationHandler>();
            
            builder.Services.AddHttpClient("ProductService", client =>
            {
                var productServiceUrl = builder.Configuration["Services:ProductService"] ?? "http://productservice:8080";
                client.BaseAddress = new Uri(productServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<ServiceAuthenticationHandler>();

            builder.Services.AddHttpClient("UserService", client =>
            {
                var userServiceUrl = builder.Configuration["Services:UserService"] ?? "http://userservice:8080";
                client.BaseAddress = new Uri(userServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<ServiceAuthenticationHandler>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
