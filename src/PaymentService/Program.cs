using PaymentService.Mappings;
using PaymentService.Data;
using Microsoft.EntityFrameworkCore;
using PaymentService.Services;
using PaymentService.Services.Providers;

namespace PaymentService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(PaymentProfile));
            builder.Services.AddSwaggerGen();

            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(conn))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. PaymentService requires a PostgreSQL connection string (set DefaultConnection in configuration or environment).");
            }

            builder.Services.AddDbContext<PaymentDbContext>(options =>
                options.UseNpgsql(conn));

            builder.Services.AddSingleton<IPaymentProvider, RealStripeProvider>();
            builder.Services.AddSingleton<IPaymentProvider, MockCreditCardProvider>();
            builder.Services.AddSingleton<IPaymentProvider, MockCashOnDeliveryProvider>();

            builder.Services.AddSingleton<PaymentProviderFactory>();

            builder.Services.AddScoped<IPaymentService, PaymentService.Services.PaymentService>();
            builder.Services.AddScoped<IStripeCheckoutService, StripeCheckoutService>();
            builder.Services.AddHttpClient();

            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
                try
                {
                    Console.WriteLine("Applying database migrations...");
                    dbContext.Database.Migrate();
                    Console.WriteLine("✅ Database migrations applied successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error applying migrations: {ex.Message}");
                }
            }
            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();

            Console.WriteLine("=== Payment Service Started ===");
            Console.WriteLine("Supported payment methods:");
            Console.WriteLine("  - STRIPE (Real - Test Mode)");
            Console.WriteLine("  - CREDIT_CARD (Mock)");
            Console.WriteLine("  - CASH_ON_DELIVERY (Mock)");
            Console.WriteLine("================================");

            app.Run();
        }
    }
}
