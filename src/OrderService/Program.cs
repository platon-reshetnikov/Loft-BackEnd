using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OrderService API", Version = "v1" });
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var cad = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (cad == null) return false;
                    return cad.ControllerTypeInfo.Assembly == typeof(Program).Assembly;
                });
                c.MapType<Microsoft.AspNetCore.Http.IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema { Type = "string", Format = "binary" });
            });
            
            builder.Services.AddDbContext<OrderDbContext>(options =>
                options.UseNpgsql(connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    }));

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
            
            builder.Services.AddTransient<ServiceAuthenticationHandler>();
            
            builder.Services.AddHttpClient("CartService", client =>
            {
                var cartServiceUrl = builder.Configuration["Services:CartService"] ?? "http://localhost:5002";
                client.BaseAddress = new Uri(cartServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<ServiceAuthenticationHandler>();
            
            builder.Services.AddHttpClient("ProductService", client =>
            {
                var productServiceUrl = builder.Configuration["Services:ProductService"] ?? "http://localhost:5001";
                client.BaseAddress = new Uri(productServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<ServiceAuthenticationHandler>();
            
            builder.Services.AddHttpClient("UserService", client =>
            {
                var userServiceUrl = builder.Configuration["Services:UserService"] ?? "http://localhost:5003";
                client.BaseAddress = new Uri(userServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<ServiceAuthenticationHandler>();
            
            builder.Services.AddHttpClient("PaymentService", client =>
            {
                var paymentServiceUrl = builder.Configuration["Services:PaymentService"] ?? "http://localhost:5005";
                client.BaseAddress = new Uri(paymentServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<ServiceAuthenticationHandler>();

            builder.Services.AddHttpClient("ShippingAddressService", client =>
            {
                var shippingServiceUrl = builder.Configuration["Services:ShippingAddressService"] ?? "http://localhost:5006";
                client.BaseAddress = new Uri(shippingServiceUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<ServiceAuthenticationHandler>();


            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseRouting();
            if (app.Services.GetService(typeof(Microsoft.AspNetCore.Authorization.IAuthorizationService)) != null)
            {
                app.UseAuthorization();
            }
             app.MapControllers();
             
             app.Run();
        }
    }
}
