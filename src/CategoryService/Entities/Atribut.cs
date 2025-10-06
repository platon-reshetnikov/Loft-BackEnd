using Loft.Common.Enums;
using ProductService.Entities;
using System.ComponentModel.DataAnnotations;

namespace CategoryService.Entities
{
    // Таблица Атрибутов
    public class Atribut
    {
        [Key]
        public int Id { get; set; }

        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!; // Название атрибута

        public AttributeType Type { get; set; }   // Тип атрибута (текст, число, список)      

        [MaxLength(50)]
        public string? TypeName { get; set; } // Название типа (если нужно кастомное)

        public string? ListOptions { get; set; } // JSON или строка с вариантами списка (например, "Red,Green,Blue")

        public ICollection<Category> Categories { get; set; } = new List<Category>(); // Многие-ко-многим с категориями (если атрибут может быть в нескольких)
    }
}
