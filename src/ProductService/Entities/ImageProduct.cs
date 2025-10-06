using Loft.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Entities
{
    public class ImageProduct
    {
        [Key]
        public long Id { get; set; } // Identity

        [Required, MaxLength(500)]
        public string Url { get; set; } = null!; // URL изображения

        public ModerationStatus Status { get; set; } = ModerationStatus.Pending; // Статус модерации

        public long ProductId { get; set; } // Внешний ключ на продукт
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!; // Навигационное свойство на продукт
    }
}
