using Microsoft.EntityFrameworkCore;
using ShippingAddressService.Data;
using ShippingAddressService.Mappings;
using ShippingAddressService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace ShippingAddressService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            
            builder.Services.AddAutoMapper(typeof(ShippingAddressProfile));

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
                { 
                    Title = "ShippingAddressService API", 
                    Version = "v1",
                    Description = "API для управления адресами доставки пользователей"
                });
                
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var cad = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (cad == null) return false;
                    return cad.ControllerTypeInfo.Assembly == typeof(Program).Assembly;
                });
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ShippingAddressDbContext>(options =>
                options.UseNpgsql(connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                    }));

            builder.Services.AddScoped<IShippingAddressService, Services.ShippingAddressService>();

            builder.Services.AddHttpClient("UserService", client =>
            {
                var userServiceUrl = builder.Configuration["Services:UserService"] ?? "http://userservice:8080";
                client.BaseAddress = new Uri(userServiceUrl);
            });

            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection.GetValue<string>("Key");
            var jwtIssuer = jwtSection.GetValue<string>("Issuer");
            var jwtAudience = jwtSection.GetValue<string>("Audience");

            if (!string.IsNullOrEmpty(jwtKey))
            {
                var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
                        ValidAudience = jwtAudience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                        ValidateLifetime = false, // Принимаем вечные токены
                        ClockSkew = TimeSpan.FromMinutes(5),
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role,
                        RequireSignedTokens = true
                    };
                });
            }

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
