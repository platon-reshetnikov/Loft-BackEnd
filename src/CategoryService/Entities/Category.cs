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

    public int? ParentId { get; set; } // Для подкатегорий
    [ForeignKey(nameof(ParentId))]
    public Category? Parent { get; set; } // Навигация к родительской категории
    public string ImgUrl { get; set; } = null!;     // URL изображения категории

    public ICollection<Category> SubCategories { get; set; } = new List<Category>(); // Подкатегории

    public ICollection<Atribut> Attributes { get; set; } = new List<Atribut>(); // Атрибуты категории (до 10, проверять в сервисе)

    public int ViewCount { get; set; } = 0; // Счётчик просмотров
}