using ProductService.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CategoryService.Entities
{
    // Таблица Атрибутов Товаров (значения)
    public class ProductAttribute
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор

        public int AttributeId { get; set; } // Внешний ключ на таблицу Атрибутов
        [ForeignKey(nameof(AttributeId))]
        public Atribut Attribute { get; set; } = null!; // Навигационное свойство к Атрибуту

        [MaxLength(500)]
        public string Value { get; set; } = null!; // Значение (строка, число или из списка; парсить по типу атрибута)

        public int ProductId { get; set; }  // Внешний ключ на таблицу Товаров
    }
}
