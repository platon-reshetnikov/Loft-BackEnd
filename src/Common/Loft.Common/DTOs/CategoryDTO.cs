using Loft.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Loft.Common.DTOs;

public class CategoryDto
{
    public int Id { get; set; } // Уникальный идентификатор
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;   // Название категории
    public int? ParentId { get; set; } // Для подкатегорий
    public string ImgUrl { get; set; } = null!;    // URL изображения категории
    public ModerationStatus Status { get; set; } = ModerationStatus.Pending;
    public int ViewCount { get; set; }  = 0; // Счётчик просмотров
    public List<int> AttributeIds { get; set; } = new List<int>(); // Список идентификаторов атрибутов категории (до 10)
}