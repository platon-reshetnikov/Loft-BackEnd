using Loft.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Entities;

public class Product
{
    [Key]
    public long Id { get; set; }    // Identity
    public long UserId { get; set; } // Владелец (пользователь)
    public long? CategoryId { get; set; } // Категория товара
    public long? ProductAttributesId { get; set; } // Атрибуты товара

    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;   // Название товара
    [Required]
    public ProductType Type { get; set; } = ProductType.Physical;  // тип твара (физический, цифровой и т.д.)
    public string? Description { get; set; }  // описание товара
    public decimal Price { get; set; } // цена
    [Required]
    public CurrencyType Currency { get; set; } = CurrencyType.UAH; // валюта
    public int ViewCount { get; set; } = 0; // Количество просмотров
    public ICollection<ImageProduct> Image { get; set; } = new List<ImageProduct>(); // Изображения
    public ICollection<CommentProduct> Comments { get; set; } = new List<CommentProduct>(); // Комментарии
    public ModerationStatus Status { get; set; } = ModerationStatus.Pending; // Статус модерации
    public int StockQuantity { get; set; } = 1; // Количество на складе
    public DateTime DateAdded { get; set; } = DateTime.UtcNow; // Дата создания
    public DateTime? UpdatedAt { get; set; } // Дата обновления
}