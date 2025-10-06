using Loft.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Entities;

public class Product
{
    [Key]
    public long Id { get; set; }    // Identity
    public long UserId { get; set; } // �������� (������������)
    public long? CategoryId { get; set; } // ��������� ������
    public long? ProductAttributesId { get; set; } // �������� ������

    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;   // �������� ������
    [Required]
    public ProductType Type { get; set; } = ProductType.Physical;  // ��� ����� (����������, �������� � �.�.)
    public string? Description { get; set; }  // �������� ������
    public decimal Price { get; set; } // ����
    [Required]
    public CurrencyType Currency { get; set; } = CurrencyType.UAH; // ������
    public int ViewCount { get; set; } = 0; // ���������� ����������
    public ICollection<ImageProduct> Image { get; set; } = new List<ImageProduct>(); // �����������
    public ICollection<CommentProduct> Comments { get; set; } = new List<CommentProduct>(); // �����������
    public ModerationStatus Status { get; set; } = ModerationStatus.Pending; // ������ ���������
    public int StockQuantity { get; set; } = 1; // ���������� �� ������
    public DateTime DateAdded { get; set; } = DateTime.UtcNow; // ���� ��������
    public DateTime? UpdatedAt { get; set; } // ���� ����������
}