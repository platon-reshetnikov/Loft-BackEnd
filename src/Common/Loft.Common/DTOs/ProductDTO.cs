using Loft.Common.Enums;

namespace Loft.Common.DTOs;

public record ProductDTO
{
    public ProductDTO() { } // пустой конструктор для AutoMapper и сериализации

    // Уникальный идентификатор продукта
    public long Id { get; set; }

    // Идентификатор продавца (владельца)
    public long SellerId { get; set; }

    // Идентификатор категории товара
    public long CategoryId { get; set; }

    // Идентификатор атрибутов товара
    public long ProductAttributesId { get; set; }

    // Название товара
    public string Name { get; set; } = string.Empty;

    // Тип товара
    public ProductType Type { get; set; }

    // Описание товара
    public string Description { get; set; } = string.Empty;

    // Цена товара
    public decimal Price { get; set; }

    // Валюта 
    public CurrencyType Currency { get; set; }

    // Количество просмотров товара
    public int ViewCount { get; set; }

    // Статус модерации продукта
    public ModerationStatus Status { get; set; }

    // Количество товара на складе
    public int StockQuantity { get; set; }

    // Список URL изображений товара
    public List<string> ImageUrls { get; set; } = new List<string>();

    // Дата добавления продукта
    public DateTime DateAdded { get; set; }

    // Дата последнего обновления продукта 
    public DateTime UpdatedAt { get; set; }
}
