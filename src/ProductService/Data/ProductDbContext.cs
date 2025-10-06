using Microsoft.EntityFrameworkCore;
using ProductService.Entities;

namespace ProductService.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<CommentProduct> CommentProduct { get; set; }
    public DbSet<ImageProduct> ImageProduct { get; set; }

    // Настройка модели и связей
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация сущности Product
        modelBuilder.Entity<Product>(entity =>
        {
            // Указываем первичный ключ
            entity.HasKey(p => p.Id);

            // Настройка обязательных полей
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Настройка перечислений (хранятся как строки в базе данных)
            entity.Property(p => p.Type)
                .IsRequired();

            entity.Property(p => p.Currency)
                .IsRequired();

            entity.Property(p => p.Status)
                .IsRequired();

            // Настройка поля Price
            entity.Property(p => p.Price)
                .HasColumnType("decimal(10,2)"); // Указываем тип decimal с точностью 10, масштабом 2

            // Настройка связи с ImageProduct (один продукт - много изображений)
            entity.HasMany(p => p.Image)
                .WithOne(ip => ip.Product)
                .HasForeignKey(ip => ip.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление продукта удаляет связанные изображения

            // Настройка связи с CommentProduct (один продукт - много комментариев)
            entity.HasMany(p => p.Comments)
                .WithOne(cp => cp.Product)
                .HasForeignKey(cp => cp.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление продукта удаляет связанные комментарии
        });

        // Конфигурация сущности ImageProduct
        modelBuilder.Entity<ImageProduct>(entity =>
        {
            // Указываем первичный ключ
            entity.HasKey(ip => ip.Id);

            // Настройка обязательных полей
            entity.Property(ip => ip.Url)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(ip => ip.ProductId)
                .IsRequired();

            entity.Property(ip => ip.Status)
                .HasConversion<string>()
                .IsRequired();
        });

        // Конфигурация сущности CommentProduct
        modelBuilder.Entity<CommentProduct>(entity =>
        {
            // Указываем первичный ключ
            entity.HasKey(cp => cp.Id);

            // Настройка обязательных полей
            entity.Property(cp => cp.Text)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(cp => cp.ProductId)
                .IsRequired();

            entity.Property(cp => cp.UserId)
                .IsRequired();

            entity.Property(cp => cp.CreatedAt)
                .IsRequired();

            entity.Property(cp => cp.Status)
                .HasConversion<string>()
                .IsRequired();
        });

        // Индексы для повышения производительности
        modelBuilder.Entity<ImageProduct>()
            .HasIndex(ip => ip.ProductId);

        modelBuilder.Entity<CommentProduct>()
            .HasIndex(cp => cp.ProductId);
    }
}