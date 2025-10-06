using Loft.Common.Enums;
using ProductService.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CategoryService.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }

    public ModerationStatus Status { get; set; } = ModerationStatus.Pending;

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    public int? ParentId { get; set; } // ��� ������������
    [ForeignKey(nameof(ParentId))]
    public Category? Parent { get; set; } // ��������� � ������������ ���������
    public string ImgUrl { get; set; } = null!;     // URL ����������� ���������

    public ICollection<Category> SubCategories { get; set; } = new List<Category>(); // ������������

    public ICollection<Atribut> Attributes { get; set; } = new List<Atribut>(); // �������� ��������� (�� 10, ��������� � �������)

    public int ViewCount { get; set; } = 0; // ������� ����������
}