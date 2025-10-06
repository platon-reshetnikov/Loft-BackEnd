using CategoryService.Entities;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Entities;

namespace CategoryService.Data;

public class CategoryDbContext : DbContext
{
    public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options)
    {
    }

    public DbSet<Atribut> Atribut { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<ProductAttribute> ProductAttribute { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            // Primary key
            entity.HasKey(c => c.Id);
            // Properties
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(c => c.ImgUrl)
                .IsRequired();
            entity.Property(c => c.Status)
                .IsRequired();
            entity.Property(c => c.ViewCount)
                .HasDefaultValue(0);
            // Self-referencing relationship for subcategories
            entity.HasOne(c => c.Parent)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid cycles
                                                    // Index for performance
            entity.HasIndex(c => c.Name);
            entity.HasIndex(c => c.ParentId);
        });

        // Configure Atribut entity
        modelBuilder.Entity<Atribut>(entity =>
        {
            // Primary key
            entity.HasKey(a => a.Id);
            // Properties
            entity.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(a => a.TypeName)
                .HasMaxLength(50);
            entity.Property(a => a.Status)
                .IsRequired();
            // Index for performance
            entity.HasIndex(a => a.Name);
        });

        // Configure ProductAttribute entity
        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            // Primary key
            entity.HasKey(pa => pa.Id);
            // Properties
            entity.Property(pa => pa.Value)
                .IsRequired()
                .HasMaxLength(500);
            // Index for performance
            entity.HasIndex(pa => pa.AttributeId);
            entity.HasIndex(pa => pa.ProductId);
        });

        // Configure many-to-many relationship between Category and Atribut
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Attributes)
            .WithMany(a => a.Categories)
            .UsingEntity<Dictionary<string, object>>("CategoryAttributes",
                j => j.HasOne<Atribut>().WithMany().HasForeignKey("AtributId"),
                j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                j =>
                {
                    j.Property<int>("CategoryId"); // явно указываем тип long
                    j.Property<int>("AtributId");   // явно указываем тип int
                    j.HasKey("CategoryId", "AtributId"); // —оставной ключ
                    j.HasIndex("CategoryId", "AtributId").IsUnique(); // ”никальный индекс
                });
    }
}