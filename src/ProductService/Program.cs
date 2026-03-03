using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductService.Data;
using ProductService.Mappings;
using ProductService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ProductDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName);
                }));

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(ProductProfile));

            builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();

            builder.Services.AddHttpClient("UserService", client =>
            {
                var userServiceUrl = builder.Configuration["Services:UserService"] ?? "http://localhost:5003";
                client.BaseAddress = new Uri(userServiceUrl);
            });
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection.GetValue<string>("Key");
            var jwtIssuer = jwtSection.GetValue<string>("Issuer");
            var jwtAudience = jwtSection.GetValue<string>("Audience");
            
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("[ProductService] JWT Key is not configured!");
            }

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
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,
                    RequireSignedTokens = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].ToString();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authHeader.Substring("Bearer ".Length).Trim();
                        }
                        Console.WriteLine($"[ProductService] Received Authorization header: {(string.IsNullOrEmpty(authHeader) ? "EMPTY" : "SET")}");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[ProductService] Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var userName = context.Principal?.Identity?.Name;
                        Console.WriteLine($"[ProductService] Token validated - UserId: {userId}, UserName: {userName}");
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductService API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token"
                };
                c.AddSecurityDefinition("bearerAuth", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var cad = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    return cad != null && cad.ControllerTypeInfo.Assembly == typeof(Program).Assembly;
                });

                c.MapType<Microsoft.AspNetCore.Http.IFormFile>(() =>
                    new OpenApiSchema { Type = "string", Format = "binary" });
            });

            var app = builder.Build();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
