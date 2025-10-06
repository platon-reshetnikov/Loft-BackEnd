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

    // ��������� ������ � ������
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ������������ �������� Product
        modelBuilder.Entity<Product>(entity =>
        {
            // ��������� ��������� ����
            entity.HasKey(p => p.Id);

            // ��������� ������������ �����
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            // ��������� ������������ (�������� ��� ������ � ���� ������)
            entity.Property(p => p.Type)
                .IsRequired();

            entity.Property(p => p.Currency)
                .IsRequired();

            entity.Property(p => p.Status)
                .IsRequired();

            // ��������� ���� Price
            entity.Property(p => p.Price)
                .HasColumnType("decimal(10,2)"); // ��������� ��� decimal � ��������� 10, ��������� 2

            // ��������� ����� � ImageProduct (���� ������� - ����� �����������)
            entity.HasMany(p => p.Image)
                .WithOne(ip => ip.Product)
                .HasForeignKey(ip => ip.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // �������� �������� ������� ��������� �����������

            // ��������� ����� � CommentProduct (���� ������� - ����� ������������)
            entity.HasMany(p => p.Comments)
                .WithOne(cp => cp.Product)
                .HasForeignKey(cp => cp.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // �������� �������� ������� ��������� �����������
        });

        // ������������ �������� ImageProduct
        modelBuilder.Entity<ImageProduct>(entity =>
        {
            // ��������� ��������� ����
            entity.HasKey(ip => ip.Id);

            // ��������� ������������ �����
            entity.Property(ip => ip.Url)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(ip => ip.ProductId)
                .IsRequired();

            entity.Property(ip => ip.Status)
                .HasConversion<string>()
                .IsRequired();
        });

        // ������������ �������� CommentProduct
        modelBuilder.Entity<CommentProduct>(entity =>
        {
            // ��������� ��������� ����
            entity.HasKey(cp => cp.Id);

            // ��������� ������������ �����
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

        // ������� ��� ��������� ������������������
        modelBuilder.Entity<ImageProduct>()
            .HasIndex(ip => ip.ProductId);

        modelBuilder.Entity<CommentProduct>()
            .HasIndex(cp => cp.ProductId);
    }
}