using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserService.Entities;
using Loft.Common.Enums;

namespace ProductService.Entities
{
    // Таблица Комментариев
    public class CommentProduct
    {
        [Key]
        public long Id { get; set; } // Identity

        [Required, MaxLength(1000)]
        public string Text { get; set; } = null!;   // Текст комментария

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Дата создания комментария

        public long UserId { get; set; }     // Владелец (пользователь)

        public ModerationStatus Status { get; set; } = ModerationStatus.Pending; // Статус модерации

        public long ProductId { get; set; }   // Внешний ключ на продукт
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;   // Навигационное свойство на продукт
    }
}
